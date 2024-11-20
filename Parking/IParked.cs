using System.Globalization;

public interface IParked
{
    IVehicle Vehicle { get; }
    string ParkingUId { get; }
    string Registration { get; }
    void StarParking();
    TimeSpan EndParking();
    TimeSpan ElapsedTime();
}


