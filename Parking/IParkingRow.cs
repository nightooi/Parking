public interface IParkingRow
{
    protected string RowEnumeration { get; }
    //0 iterated counted from left to right
    bool HasOpposing { get; }
    int Row { get; }
    //0 Iterated
    int TotalSpaces { get; }
    public bool IsTaken(int position);
    public IParkingSpace GetParking(int position);
}
