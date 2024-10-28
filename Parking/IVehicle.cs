using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
public interface IVehicle
{
    IRegistrationNumber Number { get; }
    VehicleSize Size { get; }
    Color Color { get; }
}
