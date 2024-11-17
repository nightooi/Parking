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
    }
    //Inline definition
    public ParkingSpace(string row, int numb, Occupied occupied) : this(row, numb) 
    {
        this.Status = occupied;
    }
    private string _uId;
    public string UId => _uId;
    //confusion.... was cooked or fucking what? 
    public Occupied Status { get; set; }
    public int CompareTo(IParkingSpace? other)
    {
         ValueTuple<int, int> res;
        ValueTuple<int, int> res1;
        if(other is not null && this is not null)
        {
            res = UIDToPos.UIdToPos(this._uId);
            res1 = UIDToPos.UIdToPos(other.UId);
            if(res.Item1 - res1.Item1 == 0)
            {
                return res.Item2 - res1.Item2;
            }
            return res.Item1 - res.Item2;
        }
        return this.UId.CompareTo(other.UId);

    }
}
