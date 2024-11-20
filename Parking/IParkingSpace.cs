using System.Collections;
using System.Runtime.CompilerServices;

public interface IParkingSpace : IComparer, IComparable
{
    public string UId { get; }
    Occupied Status { get; set; }
}

