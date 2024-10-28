using System.Drawing;

public abstract class Vehicle : IVehicle
{
    public Vehicle(IRegistrationNumber registration, VehicleSize size, Color color)
    {
        _regnumber = registration;
        _size = size;
        _color = color;
    }

    protected IRegistrationNumber _regnumber;
    protected VehicleSize _size;
    protected Color _color;
    public IRegistrationNumber Number => _regnumber;
    public virtual VehicleSize Size { 
        get { return _size; } 
        protected set { _size = value; }
    }
    public Color Color => _color;
}
