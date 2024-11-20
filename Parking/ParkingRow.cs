using Parking.Instantiations;
using System.Collections;
using System.Runtime.InteropServices;

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
    public ParkingRow(
        bool hasOpposing,
        int rownum,
        int totalSpaces,
        string rowEnum,
        int? opposingRow,
        int count,
         ISimpleFactory<IFactory<IEnumerable<IParkingSpace>>,
            IEnumerable<IParkingSpace>> spacefactory)
    {
        this._hasOpposing = hasOpposing;
        this._row = rownum;
        this._totalSpaces = count;
        this._rowEnumeration = rowEnum;
        this.parkingSpace = spacefactory.Create().ToArray();
        _opposingRow = opposingRow ?? -1;
        
    }
    private bool _hasOpposing;
    private readonly int _row, _totalSpaces,_opposingRow; //total spaces defines how long the row is, maxvalue of It(iterator) = _totalSpaces-1;
    private string _rowEnumeration;

    private readonly IParkingSpace[] parkingSpace;
    string RowEnumeration => _rowEnumeration;

    bool IParkingRow.HasOpposing => _hasOpposing;

    int IParkingRow.RowNumber => _row;

    int IParkingRow.OpposingRow => _opposingRow;

    int IParkingRow.TotalSpaces => _totalSpaces;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="position">Used as iterator into array, 0 iterated max = TotalSpaces-1</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">Throws if position is greater than the total len of the array.
    /// size is dictaged by config for current row</exception>
    string IParkingRow.RowEnumeration => _rowEnumeration;

    public int Count => parkingSpace.Length;

    public bool IsTaken(int position)
    {
        if (position < _totalSpaces)
        {
            return parkingSpace[position].Status == Occupied.Free;
        }
        throw new ArgumentOutOfRangeException();
    }
    IParkingSpace IParkingRow.GetParking(int position)
    {
        if (position < this.parkingSpace.Length)
        {
            return parkingSpace[position];
        }
        throw new ArgumentOutOfRangeException(string.Format("paramter {0}" +
            " was out of range", position));
    }
    public IEnumerator<IParkingSpace> GetEnumerator()
    {
        return this.parkingSpace.AsEnumerable().GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.parkingSpace.GetEnumerator();
    }
    // this needs to be nonmutable
    
}
