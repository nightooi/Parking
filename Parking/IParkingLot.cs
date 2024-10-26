public interface IParkingLot
{
    //have to return an object here somewhere in order too
    public IParkingSpace AssignSpot(IVehicle vehicle, IOwner owner);
    public void FreeSpot(IOwner owner, IVehicle vehicle);
    public void FreeSpot(IVehicle vehicle);
    public void FreeSpot(IOwner owner);
    public void FreeSpot(string uId);
    public void ClearParking();
    public void ClearRow();
}
