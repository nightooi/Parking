//Track busy Spaces
public interface IParkingTracker
{
    IParkingSpace[] BusySlots();
    IParkingSpace[] FreeSlots();
    IParkingSpace? IsFree(string uId);
}
