
using Parking.Instantiations;

using System.Collections;
using System.Runtime.CompilerServices;

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
    public void FreeSpot(IParkingSpace vehicle);
    public void FreeSpot(IParkingSpace[] vehicles);
    public void FreeSpot(string uId);
    public IEnumerable<IParkingSpace>? ClearParking();
    public bool HasOpposing(int row);
    public IParkingRow[] OpposingPairs();
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
        this._unavailable = new VehicleSize[3];
        this.rows = rowfactory.Create().ToArray();
        this.opposing = new IParkingRow[this.rows.Length];
        this.NextSpotLarge = new Stack<IParkingSpace>();
        this.NextSpotMedium = new Stack<IParkingSpace>();
        this.NextSpotSmall = new Stack<IParkingSpace>();
    }

    IParkingRow[] rows;
    IParkingRow[] opposing;
    //Hash => row, parking
    IDictionary<int, string> SingleResolver { get; set; }
    //implement a double parkingspace for easier management?
    Stack<IParkingSpace> NextSpotLarge;
    Stack<IParkingSpace> NextSpotMedium;
    Stack<IParkingSpace> NextSpotSmall;

    private IList<VehicleSize>_unavailable;
    // #TODO
    //  Will not be implemeneted for now
    //  ParkingTracker can track activity for 
    //  the parking lot and deny access to for certain types
    //  of vehicles based on some parameter like:
    //      ease of access, economics for the parkinglot
    //  
    public IReadOnlyList<VehicleSize> UnavailableFor { get => 
            (IReadOnlyList<VehicleSize>)_unavailable; }
    private IParkingSpace ResolveAppropriateSpaces(VehicleSize size)
    {
        return size switch
        {
            VehicleSize.small => HandleSmall(),
            VehicleSize.normal => NextSpotMedium.Pop(),
            VehicleSize.large => NextSpotLarge.Pop()
        }; 
    }
    private IParkingSpace HandleSmall()
    {
        if(NextSpotSmall.Peek().Status == Occupied.Free)
        {
            IParkingSpace space;
            (space = NextSpotSmall.Peek()).Status = Occupied.Half;
            return space;
        }
        return NextSpotSmall.Pop();
    }
    private void PopulateSmallParkingStack()
    {
        foreach(var row in rows)
        {
            for(int i =row.Count-1; i > 0; i--)
            {
               IParkingSpace space;
                if (((space = row[i]).Status == Occupied.Half
                    || space.Status == Occupied.Free)
                    && !NextSpotSmall.Contains(space))
                {
                    NextSpotLarge.Push(space);
                    RemoveUnavailable(VehicleSize.small);
                } 
            }
        }

        NextSpotSmall = new(NextSpotSmall.Order());
    }
    private void PopulateLargeParkingStack()
    {
        for(int i = this.opposing.Length-1; i > 0; i--)
        {
            for(int k = opposing[i].TotalSpaces-1; k > 0; k--)
            {
                int cOpp = opposing[i].OpposingRow;
                if ((opposing[i][k].Status & rows[cOpp][k].Status) == Occupied.Free
                    && !NextSpotLarge.Contains(opposing[i][k]))
                {
                    NextSpotLarge.Push(opposing[i][k]);
                    NextSpotLarge.Push(rows[cOpp][k]);
                    RemoveUnavailable(VehicleSize.large);
                }
            }
        }
        NextSpotLarge = new(NextSpotLarge.Order());
    }
    private void PopulateNormalParkingStack()
    {
        foreach(var row in rows)
        {
            for(int i = row.TotalSpaces-1; i >= 0; i--)
            {
                if (row.GetParking(i).Status == Occupied.Free
                    && !NextSpotMedium.Contains(row.GetParking(i)))
                {
                    NextSpotLarge.Push(row.GetParking(i));
                    RemoveUnavailable(VehicleSize.normal);
                }
            }
        }

        NextSpotMedium = new(NextSpotMedium.Order());
    }
    private void RemoveUnavailable(VehicleSize size)
    {
        if (_unavailable.Contains(size)) _unavailable.Remove(size);
    }
    private IEnumerable<IParkingRow> FindLargeParkingSpace()
    {

        for (int i = rows.Length - 1; i > 0; i--)
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
    private VehicleSize? IsUnaVailable(IVehicle vehicle)
    {
        if (_unavailable.Contains(vehicle.Size))
        {
            return vehicle.Size;
        }
        switch (vehicle.Size)
        {
            case VehicleSize.large:
                if (!_unavailable.Contains(VehicleSize.large) 
                    && NextSpotLarge.Count <= 1)
                {
                    _unavailable.Add(VehicleSize.large);
                    return VehicleSize.large;
                }
                break;
            case VehicleSize.normal:
                if (!_unavailable.Contains(VehicleSize.normal)
                    && NextSpotMedium.Count <= 1)
                {
                    _unavailable.Add(VehicleSize.normal);
                    return VehicleSize.normal;
                }
                break;
            case VehicleSize.small:
                if (NextSpotSmall.Count <= 1
                    && !_unavailable.Contains(VehicleSize.large))
                {
                    _unavailable.Add(VehicleSize.normal);
                    return VehicleSize.small;
                }
                break;
        }
        return null;
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
    public void FreeSpot(string uId)
    {
        var res = UIDToPos.UIdToPos(uId);
        rows[res.Item1][res.Item2].Status = Occupied.Free;
        PopulateLargeParkingStack();
        PopulateNormalParkingStack();
        PopulateSmallParkingStack();
    }
    //first and second element tied together as a pair.
    public IParkingRow[] OpposingPairs()
    {
        IParkingRow[] rows = new IParkingRow[this.rows.Length];
        int i = 0;
        for(int k = 0; k < this.rows.Length; i++)
        {
            if (this.rows[k].HasOpposing && (++k < rows.Length))
            {
                rows[i] = this.rows[k];
                k++;
                i++;
                rows[i] = this.rows[k];
            }
            i++;
        }
        return rows;
    }
    public void FreeSpot(IParkingSpace space)
    {
        FreeSpot(space.UId);
    }
    public void FreeSpot(IParkingSpace[] vehicles)
    {
        foreach(var vehicle in vehicles)
        {
            FreeSpot(vehicle.UId);
        }
    }
}
public static class UIDToPos 
{ 
    public static (int, int) UIdToPos(string uid)
    {
        var row = uid[0];
        int numbInRow = 0;
        if(int.TryParse(uid.Substring(1, uid.Length),out numbInRow))
        {
           int numbRow = (int)uid[0] - 65; //A utf-16 base 10 is 65;
            return (numbRow, numbInRow);
        }
        throw new ApplicationException("Naming schema for rows is misconfigured!");
    }
}












