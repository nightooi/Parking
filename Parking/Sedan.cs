using System.Drawing;

public sealed class Sedan : Vehicle
{
    public override VehicleSize Size { 
        get{
            return _size;
        }
        protected set { 
            if(!(value == VehicleSize.normal))
            {
                throw new Exception("Overriding VehicleSize");
            }
            _size = value;
        }
    }
    public Sedan(string registration,  ConsoleColor color) : base(registration, VehicleSize.normal, color)
    {

    }
}
