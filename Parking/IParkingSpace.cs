using System.Runtime.CompilerServices;

public interface IParkingSpace : IComparable<IParkingSpace>
{
    public string UId { get; }
    Occupied Status { get; set; }
}