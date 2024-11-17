using System.Globalization;

public interface IParked
{
    string ParkingUId { get; }
    IOwner Owner { get; }
    IRegistrationNumber Registration { get; }
    void StarParking();
    TimeSpan EndParking();
}


