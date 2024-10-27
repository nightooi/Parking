
/// <summary>
/// HasOpposing will be inferred from rowEnumeration 
/// based off of the config
/// </summary>
public class ParkingRow : IParkingRow
{
    /// <summary>
    /// Supply all parameters from config.
    /// </summary>
    /// <param name="hasOpposing"></param>
    /// <param name="rownum"></param>
    /// <param name="totalSpaces"></param>
    /// <param name="rowEnum"></param>
    public ParkingRow(bool hasOpposing, int rownum, int totalSpaces, string rowEnum)
    {
        this._hasOpposing = hasOpposing;
        this._row = rownum;
        this._totalSpaces = totalSpaces;
        this._rowEnumeration = rowEnum;
        this.it = totalSpaces - 1;
        this.parkingSpace = ConstructRow();
    }
    private bool _hasOpposing;
    private int _row, _totalSpaces, it; //total spaces defines how long the row is, maxvalue of It(iterator) = _totalSpaces-1;
    private string _rowEnumeration;

    private readonly IParkingSpace[] parkingSpace;
    string IParkingRow.RowEnumeration => _rowEnumeration;
    public bool HasOpposing => _hasOpposing;
    public int Row => _row;
    public int TotalSpaces => _totalSpaces;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">Throws if position is greater than the total len of the array.
    /// size is dictaged by config for current row</exception>
    public IParkingSpace GetParking(int position)
    {
        if(position < _totalSpaces)
        {
            return parkingSpace[position];
        }
        throw new ArgumentOutOfRangeException();
    }
    public bool IsTaken(int position)
    {
        if(position < _totalSpaces)
        {
            return parkingSpace[position].Status == Occupied.Free;
        }
        throw new ArgumentOutOfRangeException();
    }
    private IParkingSpace[] ConstructRow()
    {
        IParkingSpace[] space = new IParkingSpace[_totalSpaces];
        for(int i = 0; i < it; i++)
        {
            //Im crying, i need a factory pattern, badly.
            space[i] = new ParkingSpace(string.Format("{0}:{1}",Row.ToString(),this._rowEnumeration), i);
        }
        return space;
    }
}
