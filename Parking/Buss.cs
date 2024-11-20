using System.Drawing;

public class Buss : Vehicle
{
    public override VehicleSize Size { 
        get{
            return _size;
        }
        protected set { 
            if(!(value == VehicleSize.large))
            {
                throw new Exception("Overriding VehicleSize");
            }
            _size = value;
        }
    }
    public Buss(string registration, ConsoleColor color) : base(registration, VehicleSize.large, color)
    {

    }
}
