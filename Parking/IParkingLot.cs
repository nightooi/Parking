
using Parking;
using Parking.Instantiations;

using System.Collections;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

//To be Generic constraint definer.
public interface IParkingLotOptimizer
{

}
//handles only the parkinglot and status of spaces
//this should have a facade, it really should'nt return the parkingspace instantiation.. alas
public interface IParkingLot
{
    public IReadOnlyList<VehicleSize> UnavailableFor { get; }
    //have to return an object here somewhere in order too
    public IParkingSpace AssignSpot(IVehicle vehicle);
    public void FreeSpot(IVehicle vehicle, string uid);
    public IEnumerable<IParkingSpace>? ClearParking();
    public bool HasOpposing(int row);
    public IParkingRow[] OpposingPairs();
    public bool IsUnavailable(VehicleSize size);
}

// so there are multiple way to implement this, but this type really needs a 
// templating to resolve which way its being implemented
// alas templates are out of the scope for this solution.
internal class Parkinglot : IParkingLot
{
    public Parkinglot(
        ISimpleFactory<IFactory<IEnumerable<IParkingRow>>,
            IEnumerable<IParkingRow>> rowfactory
    )
    {
        this._unavailable = new List<VehicleSize>();
        this.rows = rowfactory.Create().ToArray();
        this.opposing = new List<IParkingRow>(rows.Count);
        this.NextSpot = new();
        this.opposing = PopulateOpposing().ToArray();
        SimpleSync();
   }

    IList<IParkingRow> rows;
    IList<IParkingRow> opposing;
    //Hash => row, parking
    //implement a double parkingspace for easier management?
    List<IParkingSpace> NextSpot;

    private IList<VehicleSize>_unavailable;
    public IReadOnlyList<VehicleSize> UnavailableFor { get => 
            (IReadOnlyList<VehicleSize>)_unavailable; }
    //
    //Entirely possible to do with one stack, this approach is just 
    //too convinient when it comes to handinglig large and small vehicles.
    //
    private IParkingSpace ResolveAppropriateSpaces(VehicleSize size)
    {
        return size switch
        {
            VehicleSize.small => HandleSmall(),
            VehicleSize.normal => HandleMedium(),
            VehicleSize.large => HandleLarge(),
            _ => throw new Exception()
        };
    }
    private void SimpleSync()
    {
        PopulateNormalParkingStack();
    }
    private IEnumerable<IParkingRow> PopulateOpposing()
    {
        foreach(var row in rows)
        {
            if(row.HasOpposing && row.OpposingRow > -1)
            {
                yield return row;
            }
        }
    }
    private IParkingSpace HandleSmall()
    {
        IParkingSpace res;
        res = NextSpot.FirstOrDefault(x => x.Status == Occupied.Half);
        if(res is not null)
        {
            res.Status = Occupied.Full;
            NextSpot.Remove(res);
            return res;
        }
        res = NextSpot.First(x => x.Status == Occupied.Free);
        res.Status = Occupied.Half;
        return res;
    }
    private void CheckAvailibility()
    {
        IParkingSpace? space;
        space = NextSpot.FirstOrDefault(
        x => opposing.Any(y => y.Contains(x) &&
            x.Status == Occupied.Free
            && GetOpposing(x).Status == Occupied.Free)
        &&!_unavailable.Contains(VehicleSize.large));
        if (space is null) _unavailable.Add(VehicleSize.large);

            space = NextSpot.FirstOrDefault(x => x.Status == Occupied.Free);
        if (space is null) _unavailable.Add(VehicleSize.normal);

            space = NextSpot.FirstOrDefault(x => x.Status == Occupied.Free || x.Status ==Occupied.Half);
        if (space is null) _unavailable.Add(VehicleSize.small);

    }
    private IParkingSpace HandleMedium()
    {
        IParkingSpace res;
        if (NextSpot.First().Status == Occupied.Free)
        {
            NextSpot.First().Status = Occupied.Full;
            res = NextSpot.First();
            NextSpot.Remove(res);
            return res;
        }
        return Taken();
    }
    private IParkingSpace Taken()
    {
        IParkingSpace space = NextSpot.First(x => x.Status == Occupied.Free);
        space.Status = Occupied.Full;
        NextSpot.Remove(space);
        return space;
    }
    private IParkingSpace HandleLarge()
    {
        return TakenLarge();
    }
    private IParkingSpace TakenLarge()
    {
        IParkingSpace space;
        IParkingSpace oppSpace;
        (space = NextSpot.First(
            x => opposing.Any(y => y.Contains(x) &&
        x.Status == Occupied.Free
        && GetOpposing(x).Status == Occupied.Free))).Status = Occupied.Free;
        (oppSpace = GetOpposing(space)).Status = Occupied.Full;
        space.Status = Occupied.Full;
        NextSpot.Remove(space);
        NextSpot.Remove(oppSpace);
        return space;
    }
    private IParkingSpace GetOpposing(IParkingSpace space)
    {
        var res = UIDToPos.UIdToPos(space.UId);
        int i = ++res.Item1;
        return this.rows[i][res.Item2];
    }
    private void PopulateNormalParkingStack()
    {
        CompareByUID comp = new();
        for(int k =0; k < rows.Count; k++)
        {
            for(int i = rows[k].TotalSpaces-1; i >= 0; i--)
            {
                if (Array.BinarySearch(NextSpot.ToArray(), rows[k].GetParking(i), comp) < 0 &&
                    rows[k].GetParking(i).Status == Occupied.Free ||
                    rows[k].GetParking(i).Status == Occupied.Half)
                {
                    NextSpot.Add(rows[k].GetParking(i));
                    _unavailable.Clear();
                }
            }
        }
        NextSpot.Sort();
    }
    private IEnumerable<IParkingRow> FindLargeParkingSpace()
    {

        for (int i = rows.Count - 1; i > 0; i--)
        {
            if (rows[i].HasOpposing && (--i > 0))
            {
                yield return rows[i];
                i--;
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="vehicle"> incoming request</param>
    /// <exception cref="InvalidOperationException">
    /// Throws if there aren't any places for the vehicle,
    /// please check availability first before calling. </exception>
    /// <returns>Appropriate ParkingSpace for vehicle.</returns>
    public IParkingSpace AssignSpot(IVehicle vehicle)
    {
        return ResolveAppropriateSpaces(vehicle.Size);
    }
    public bool IsUnavailable(VehicleSize size)
    {
        CheckAvailibility();
        return _unavailable.Contains(size);
    }
    /// <summary>
    /// Clears Parking of all Vehicles!
    /// returns ALL cleared parkings!
    /// caller resolves customer currectly by UID!
    /// </summary>
    public IEnumerable<IParkingSpace>? ClearParking()
    {
        foreach(var row in rows)
        {
            for(int i = 0; i < row.Count; i++)
            {
                if (row[i].Status == Occupied.Half || row[i].Status == Occupied.Full)
                    yield return row[i];
            }
        }
    }
    public bool HasOpposing(int row)
    {
        return rows[row].HasOpposing;
    }
    private void FreeSpot(string uId, Occupied? status)
    {
        var res = UIDToPos.UIdToPos(uId);
        rows[res.Item1][res.Item2].Status = status ??= Occupied.Free;
        PopulateNormalParkingStack();
    }
    //first and second element tied together as a pair.
    public IParkingRow[] OpposingPairs()
    {
        IParkingRow[] rows = new IParkingRow[this.rows.Count];
        int i = 0;
        for(int k = 0; k < this.rows.Count; k++)
        {
            if (this.rows[k].HasOpposing)
            {
                rows[i] = this.rows[k];
                i++;
                rows[i] = this.rows[this.rows[k].OpposingRow];
            }
            i++;
        }
        return rows;
    }
    private void FreeSpot(IParkingSpace space)
    {
        FreeSpot(space.UId, null);
    }
    private IParkingSpace GetSpaceFromUID(string space)
    {
        var res =UIDToPos.UIdToPos(space);
        return this.rows[res.Item1][res.Item2];
    }
    public void FreeSpot(IVehicle vehicle, string uid)
    {
        var space = GetSpaceFromUID(uid);
        if(vehicle.Size == VehicleSize.large)
        {
            var opp =GetOpposing(space);
            FreeSpot(opp.UId, null);
            FreeSpot(space.UId, null);
            return;
        }
        else if(vehicle.Size == VehicleSize.small && space.Status == Occupied.Half)
        {
            FreeSpot(space);
            return;
        }
        else if (vehicle.Size == VehicleSize.small && space.Status == Occupied.Full)
        {
            FreeSpot(space.UId, Occupied.Half);
            return;
        }
        FreeSpot(space.UId,null);
    }
}
public static class UIDToPos 
{ 
    public static (int, int) UIdToPos(string uid)
    {   
        //unclear why but this throws sometimes, seemingly randomly
        //the only way this happens is if for some reason the UID string is malformed
        //if this throws, your guess is as good as mine at this point..

        try
        {

            var row = uid[0];
            int numbInRow = 0;
            if (int.TryParse(uid.Substring(1, uid.Length - 1), out numbInRow))
            {
                int numbRow = (int)uid[0] - 65; //A utf-16 base 10 is 65;
                return (numbRow, numbInRow);
            }
        }
        catch(Exception e)
        {
            Console.WriteLine(e.InnerException.Message + " ::::\n "+e.Message+ " :: \n" + e.StackTrace);
        }
        throw new ApplicationException("Naming schema for rows is misconfigured!");
    }
}


public class ParkingStringComparer : IComparer<IParkingSpace>, IComparer
{
    public int Compare(IParkingSpace? x, IParkingSpace? y)
    {
        if(x is not null && y is not null)
        {
            return x.UId.CompareTo(y.UId);
        }
        if(x is null && y is not null)
        {
            return -1;
        }
        if(x is not null)
        {
            return 1;
        }
        return 0;
    }

    public int Compare(object? x, object? y)
    {
        return Compare((IParkingSpace)x, (IParkingSpace)y);
    }
}
