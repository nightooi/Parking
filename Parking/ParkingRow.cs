
using Microsoft.VisualBasic;

using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Transactions;

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
        int opposingRow)
    {
        this._hasOpposing = hasOpposing;
        this._row = rownum;
        this._totalSpaces = totalSpaces;
        this._rowEnumeration = rowEnum;
        this.it = totalSpaces - 1;
        this.parkingSpace = ConstructRow();
    }
    private bool _hasOpposing;
    private readonly int _row, _totalSpaces, it, _opposingRow; //total spaces defines how long the row is, maxvalue of It(iterator) = _totalSpaces-1;
    private string _rowEnumeration;

    private readonly IParkingSpace[] parkingSpace;
    string RowEnumeration => _rowEnumeration;

    bool IParkingRow<IParkingSpace>.HasOpposing => _hasOpposing;

    int  IParkingRow<IParkingSpace>.RowNumber => _row;

    int  IParkingRow<IParkingSpace>.OpposingRow => _opposingRow;

    int  IParkingRow<IParkingSpace>.TotalSpaces => _totalSpaces;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="position">Used as iterator into array, 0 iterated max = TotalSpaces-1</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">Throws if position is greater than the total len of the array.
    /// size is dictaged by config for current row</exception>
    public IParkingSpace GetParking(int position)
    {
            }
    string IParkingRow<IParkingSpace>.RowEnumeration => _rowEnumeration;

    public int Count => throw new NotImplementedException();

    public bool IParkingRow<IParkingSpace>.IsTaken(int position)
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
        return space;
    }

    IParkingSpace IParkingRow<IParkingSpace>.GetParking(int position)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<IParkingSpace> GetEnumerator()
    {
        return this.parkingSpace.AsEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.parkingSpace.GetEnumerator();
    }

    public interface ISimpleFactory<T, U> where T : IFactory<U>
    { 
        static T Factory { get; }
        static virtual U Create()
        {
            return Factory.Imp.Invoke(Factory.parameters);
        }
        static virtual U Create(params object[] parameters)
        {
            return Factory.Imp.Invoke(new[] {parameters, Factory.parameters});
        }
    }
    public interface IFactory<T>
    {
        public object[] parameters { get; }
        public Func<object[], T> Imp { get; }
    }
    public partial class RowFactory : IFactory<IParkingSpace>
    {
        public object[] parameters => GetConfigValues();
        private Func<object[], IParkingSpace> _impl;
        public Func<object[], IParkingSpace> Imp => 
        
        public RowFactory(Func<object[], IParkingSpace> func,
            IConfig<IParkingSpace> config)
        {
            
        }
        private object[] GetConfigValues()
        {
            
        }   
    }
}
