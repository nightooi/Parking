using System.Text.Json.Serialization;
public class ParkingSpace : IParkingSpace
{
    //copy constructor
    public ParkingSpace(ParkingSpace space) 
    {
        this._uId = space._uId;
        this.Status = space.Status;
    }
    //base init
    public ParkingSpace(string row, int numb)
    {
        this.Status = Occupied.Free;
       this._uId = row + numb.ToString();
        this.Status = Occupied.Free;
    }
    //Inline definition
    public ParkingSpace(string row, int numb, Occupied occupied) : this(row, numb) 
    {
        this.Status = occupied;
    }
    private string _uId;
    public string UId => _uId;
    public Occupied Status { get; set; }

    public int Compare(object? x, object? y)
    {
        if(x is not null && y is not null)
        {
            CompareByUID comparer = new();
            return comparer.Compare((ParkingSpace)x, (ParkingSpace)y);
        }
        if (x is null && y is not null) return 1;
        if (y is null && x is not null) return -1;
        if (x is null && y is not null) return 0;
        return 0;
    }

    public int CompareTo(object? obj)
    {
        return Compare(this, obj);
    }
}
