public class ParkingSpace : IParkingSpace
{
    private Occupied _Previous;
    private Occupied _occupied;
    private string _uId;
    public string UId => _uId;
    //Statefull property... :)
    public Occupied Status { 
        get {
            return _occupied;
        }
        set
        {
            switch(_occupied)
            {
                case Occupied.Half:
                    if (value == Occupied.Half) {
                        _Previous = Occupied.Half;
                        _occupied = Occupied.Full;
                    }
                    else if(value == Occupied.Free)
                    {
                        _Previous = _occupied;
                        _occupied = Occupied.Free;
                    }
                    else
                    {
                        throw new Exception(
                            string.Format(
                            "Field Status::" +
                            " is being Set to value::: {0} which is not" +
                            " possible while the Property has the value" +
                            " {1}", value.ToString(), _occupied.ToString()));
                    }
                    break;

                case Occupied.Full:
                    if(value == Occupied.Free && _Previous == Occupied.Half)
                    {
                        _Previous = _occupied;
                        _occupied = Occupied.Half;
                    }
                    else if(value == Occupied.Free)
                    {
                        _Previous = _occupied;
                        _occupied = Occupied.Free;
                    }
                    else if (value == Occupied.Half && _Previous == Occupied.Half)
                    {
                        _Previous = _occupied;
                        _occupied = Occupied.Full;
                    }
                    else
                    {
                        throw new Exception(
                            string.Format(
                            "Field Status::" +
                            " is being Set to value::: {0} which is not" +
                            " possible while the Property has the value" +
                            " {1}", value.ToString(), _occupied.ToString()));
                    }
                    break;
                case Occupied.Free:
                    if(value != Occupied.Free) _occupied = value;
                    else throw new Exception(
                            string.Format(
                            "Field Status::" +
                            " is being Set to value::: {0} which is not" +
                            " possible while the Property has the value" +
                            " {1}", value.ToString(), _occupied.ToString()));
                    break;
            }
        }
    }
}
