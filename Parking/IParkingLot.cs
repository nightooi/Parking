
using System.Collections;
using System.Runtime.CompilerServices;

//To be Generic constraint definer.
public interface IParkingLotOptimizer
{

}
public interface IParkingLot
{
    public VehicleSize[] UnavailableFor { get; }
    //have to return an object here somewhere in order too
    public IParkingSpace AssignSpot(IVehicle vehicle, IOwner owner);
    public void FreeSpot(IOwner owner, IVehicle vehicle);
    public void FreeSpot(IVehicle vehicle);
    public IVehicle[] FreeSpot(IOwner owner);
    public void FreeSpot(string uId);
    public void ClearParking();
    public void ClearRow(string row);
    public bool IsRowClear(int row);
    public bool HasOpposing(int row);
    public IParkingRow[] OpposingPairs();
}

// so there are multiple way to implement this, but this type really needs a 
// templating to resolve which way its being implemented
// alas templates are out of the scope for this solution.
internal class Parkinglot : IParkingLot
{
    IParkingRow[] rows;
    IParkingRow[] opposing;

    // this type doesnt need Parked. it's only relevant to the Payment really.
    IList<IParked> Parked;
    //implement a double parkingspace for easier management?
    Stack<IParkingSpace> NextSpotLarge;
    Stack<IParkingSpace> NextSpotMedium;
    Stack<IParkingSpace> NextSpotSmall;

    private VehicleSize[] _unavailable;
    // #TODO
    //  Will not be implemeneted for now
    //  ParkingTracker can track activity for 
    //  the parking lot and deny access to for certain types
    //  of vehicles based on some parameter like:
    //      ease of access, economics for the parkinglot
    //
    public VehicleSize[] UnavailableFor { get => _unavailable; }
    private IParkingSpace ResolveAppropriateSpaces(VehicleSize size)
    {
        return size switch
        {
            VehicleSize.small => NextSpotSmall.Pop(),
            VehicleSize.normal => NextSpotMedium.Pop(),
            VehicleSize.large => NextSpotLarge.Pop()
        }; 
    }
    private void PopulateSmallParkingStack()
    {
        foreach(var row in rows)
        {
            for(int i =row.TotalSpaces-1; i >= 0; i--)
            {
               IParkingSpace space;
                if (((space = row.GetParking(i)).Status == Occupied.Half
                    || space.Status == Occupied.Free)
                    && !NextSpotSmall.Contains(space))
                {
                    NextSpotLarge.Push(space);
                } 
            }
        }
    }
    private void PopulateLargeParkingStack()
    {
        for(int i = this.opposing.Length-1; i >= 0; i--)
        {
            for(int k = opposing[i].TotalSpaces-1; k >= 0; k--)
            {
                int cOpp = opposing[i].OpposingRow;
                if ((opposing[i].GetParking(k).Status | rows[cOpp].GetParking(k).Status) == Occupied.Free
                    && !NextSpotLarge.Contains(opposing[i].GetParking(k)))
                {
                    NextSpotLarge.Push(opposing[i].GetParking(k));
                    NextSpotLarge.Push(rows[cOpp].GetParking(k));
                }
            }
        }
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
                }
            }
        }
    }
    private IEnumerable<IParkingRow> FindLargeParkingSpace()
    {

        for(int i =rows.Length-1; i >= 0; i--)
        {
            if (rows[i].HasOpposing)
            {
                i--; //skip over one since we know that next one is connected to this row.
                yield return rows[i];
            }
        }
    }
        
    public IParkingSpace AssignSpot(IVehicle vehicle, IOwner owner)
    {
        for (int i = 0; i < rows.Length; i++)
        {
            
        }
    }
    public void ClearParking()
    {
        throw new NotImplementedException();
    }
    public void ClearRow(string row)
    {
        throw new NotImplementedException();
    }
    public void FreeSpot(IOwner owner, IVehicle vehicle)
    {
        throw new NotImplementedException();
    }
    public void FreeSpot(IVehicle vehicle)
    {
        throw new NotImplementedException();
    }
    public IVehicle[] FreeSpot(IOwner owner)
    {
        throw new NotImplementedException();
    }
    public void FreeSpot(string uId)
    {
        throw new NotImplementedException();
    }
    public bool HasOpposing(int row)
    {
        throw new NotImplementedException();
    }
    public bool IsRowClear(int row)
    {
        throw new NotImplementedException();
    }
    public IParkingRow[] OpposingPairs()
    {
        throw new NotImplementedException();
    }
}












