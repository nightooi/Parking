
using Parking.Instantiations;

using System.Collections;
using System.ComponentModel.Design;
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
    public void FreeSpot(IVehicle vehicle, string uid);
    public IEnumerable<IParkingSpace>? ClearParking();
    public bool HasOpposing(int row);
    public IParkingRow[] OpposingPairs();
    VehicleSize? IsUnavailable(VehicleSize size);
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
        this.NextSpotLarge = new Stack<IParkingSpace>();
        this.NextSpotMedium = new Stack<IParkingSpace>();
        this.NextSpotSmall = new Stack<IParkingSpace>();
        this.opposing = PopulateOpposing().ToArray();
        SimpleSync();
   }

    IList<IParkingRow> rows;
    IList<IParkingRow> opposing;
    //Hash => row, parking
    //implement a double parkingspace for easier management?
    Stack<IParkingSpace> NextSpotLarge;
    Stack<IParkingSpace> NextSpotMedium;
    Stack<IParkingSpace> NextSpotSmall;

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
        PopulateLargeParkingStack();
        PopulateNormalParkingStack();
        PopulateSmallParkingStack();
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
        if(NextSpotSmall.Peek().Status == Occupied.Free)
        {
            (res = NextSpotSmall.Peek()).Status = Occupied.Half;
        }
        else if((res =NextSpotSmall.Peek()).Status == Occupied.Half)
        {
            res.Status = Occupied.Full;
        }
        this.CheckAvailibilit();
        return res;
    }
    private IParkingSpace HandleMedium()
    {
        IParkingSpace res;
        if(NextSpotMedium.Peek().Status == Occupied.Free)
        {
            (res =NextSpotMedium.Peek()).Status = Occupied.Full;
            this.CheckAvailibilit();
            return res;
        }
        return HandleMedium();
    }
    private IParkingSpace HandleLarge()
    {
        IParkingSpace space;
        IParkingSpace oppSpace;
        if ((space = NextSpotLarge.Peek()).Status != Occupied.Free
            || (oppSpace = GetOpposing(space)).Status != Occupied.Free)
        {
            NextSpotLarge.Pop();
        }
        else if(NextSpotLarge.Peek().Status == Occupied.Free 
            && GetOpposing(space).Status == Occupied.Free)
        {
            GetOpposing(space = NextSpotLarge.Peek()).Status = Occupied.Full;
            space.Status = Occupied.Full;
            CheckAvailibilit();
            return space;
        }
        return HandleLarge();
    }
    private IParkingSpace GetOpposing(IParkingSpace space)
    {
        var res = UIDToPos.UIdToPos(space.UId);
        int i = ++res.Item1;
        return this.rows[i][res.Item2];
    }
    private void PopulateSmallParkingStack()
    {
        foreach(var row in rows)
        {
            for(int i =row.Count-1; i >= 0; i--)
            {
               IParkingSpace space;
                if (((space = row[i]).Status == Occupied.Half
                    || space.Status == Occupied.Free)
                    && !NextSpotSmall.Contains(space))
                {
                    NextSpotSmall.Push(space);
                    RemoveUnavailable(VehicleSize.small);
                } 
            }
        }
        var res = NextSpotSmall.ToArray();
        Array.Sort(res);
        NextSpotSmall = new(res);
    }
    private void PopulateLargeParkingStack()
    {
        for(int i = this.opposing.Count-1; i >= 0; i--)
        {
            for(int k = opposing[i].TotalSpaces-1; k >= 0; k--)
            {
                int cOpp = opposing[i].OpposingRow;
                if ((opposing[i][k].Status | rows[cOpp][k].Status) == Occupied.Free
                    && !NextSpotLarge.Contains(opposing[i][k]))
                {
                    NextSpotLarge.Push(opposing[i][k]);
                    RemoveUnavailable(VehicleSize.large);
                }
            }
        }
        var res =NextSpotLarge.ToArray();
        Array.Sort(res);
        NextSpotLarge = new(res);
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
                    NextSpotMedium.Push(row.GetParking(i));
                    RemoveUnavailable(VehicleSize.normal);
                }
            }
        }
        var res = NextSpotMedium.ToArray();
        Array.Sort(res);
        NextSpotMedium = new Stack<IParkingSpace>(res);
    }
    private void RemoveUnavailable(VehicleSize size)
    {
        int[] where = new int[_unavailable.Count];
        for(int i = _unavailable.Count -1; i >= 0; i --)
        {
            if (_unavailable[i] == size)
            {
                where[i] = i;
            }
        }
        for(int i = where.Length-1; i >= 0; i--)
        {
            _unavailable.RemoveAt(i);
        }
    }
    private void CheckAvailibilit()
    {
        List<IParkingSpace> cache = new();
        while(NextSpotSmall.Count > 0 &&
            !(NextSpotSmall.Peek().Status == Occupied.Free))
        {
            if(NextSpotSmall.Peek().Status == Occupied.Half)
            {
                cache.Add(NextSpotSmall.Pop());
                continue;
            }
            NextSpotSmall.Pop();
        }
        for(int i = cache.Count-1; i >= 0; i--)
        {
            NextSpotSmall.Push(cache[i]);
        }
        UpdateOther(NextSpotMedium);
        while(NextSpotLarge.Count > 0 && 
            NextSpotLarge.Peek().Status != Occupied.Free
            || GetOpposing(NextSpotLarge.Peek()).Status != Occupied.Free)
        {
            NextSpotLarge.Pop();
        }
    }
    private void UpdateOther(Stack<IParkingSpace> stack)
    {
        while(stack.Count > 0 && 
            stack.Peek().Status != Occupied.Free)
        {
            stack.Pop();
        }
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
    public VehicleSize? IsUnavailable(VehicleSize size)
    {
        if (_unavailable.Contains(size))
        {
            return size;
        }
        switch (size)
        {
            case VehicleSize.large:
                if( NextSpotLarge.Count < 1)
                {
                    _unavailable.Add(VehicleSize.large);
                    return VehicleSize.large;
                }
                break;
            case VehicleSize.normal:
                if (NextSpotMedium.Count < 1) 
                {
                    _unavailable.Add(VehicleSize.normal);
                    return VehicleSize.normal;
                }
                break;
            case VehicleSize.small:
                if (NextSpotSmall.Count < 1 )
                {
                    _unavailable.Add(VehicleSize.small);
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
    private void FreeSpot(string uId, Occupied? status)
    {
        var res = UIDToPos.UIdToPos(uId);
        rows[res.Item1][res.Item2].Status = status ??= Occupied.Free;
        PopulateLargeParkingStack();
        PopulateNormalParkingStack();
        PopulateSmallParkingStack();
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
        }
        else if(vehicle.Size == VehicleSize.small && space.Status == Occupied.Half)
        {
            FreeSpot(space);
        }
        else if (vehicle.Size == VehicleSize.small && space.Status == Occupied.Full)
        {
            FreeSpot(space.UId, Occupied.Half);
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












