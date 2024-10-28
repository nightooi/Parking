using System.Drawing;

public static class VehicleFactory
{
    public static Vehicle Create(VehicleSize size, IRegistrationNumber number, Color color){
        return size switch
        {
            VehicleSize.large => new Buss(number, color),
            VehicleSize.normal => new Sedan(number, color),
            VehicleSize.small => new Bike(number, color),
            _ => throw new Exception("type not implemented")
        }; 
    } 
} 