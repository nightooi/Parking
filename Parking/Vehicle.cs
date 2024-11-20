using System.Drawing;

public abstract class Vehicle : IVehicle
{
    public Vehicle(string registration, VehicleSize size, ConsoleColor color)
    {
        _regnumber = registration;
        _size = size;
        _color = color;
    }

    protected string _regnumber;
    protected VehicleSize _size;
    protected ConsoleColor _color;
    public string Number => _regnumber;
    public virtual VehicleSize Size { 
        get { return _size; } 
        protected set { _size = value; }
    }
    public ConsoleColor Color => _color;
}
