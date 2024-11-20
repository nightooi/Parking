using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
public interface IVehicle
{
    string Number { get; }
    VehicleSize Size { get; }
    ConsoleColor Color { get; }
}
