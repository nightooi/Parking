using System.Drawing;

public interface IVehicle
{
    IRegistrationNumber Number { get; }
    VehicleSize Size { get; }
    Color Color { get; }
}
