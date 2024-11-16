using System.Runtime.CompilerServices;

public interface IParkingSpace
{
    public string UId { get; }
    Occupied Status { get; set; }
    public static implicit operator bool(IParkingSpace b) => (b.Status == 0);
}