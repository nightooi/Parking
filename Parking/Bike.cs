using System.Drawing;

public class Bike : Vehicle
{
    public override VehicleSize Size { 
        get{
            return _size;
        }
        protected set { 
            if(!(value == VehicleSize.small))
            {
                throw new Exception("Overriding VehicleSize");
            }
            _size = value;
        }
    }
    public Bike(IRegistrationNumber registration, Color color) : base(registration, VehicleSize.small, color)
    {

    }
}
