
using Microsoft.VisualBasic;
using Microsoft.Win32.SafeHandles;

using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Transactions;

/// <summary>
/// HasOpposing will be inferred from rowEnumeration 
/// based off of the config
/// </summary>
public class ParkingRow : IParkingRow<IParkingSpace>
{
    /// <summary>
    /// Supply all parameters from config.
    /// </summary>
    /// <param name="hasOpposing"></param>
    /// <param name="rownum"></param>
    /// <param name="totalSpaces"></param>
    /// <param name="rowEnum"></param>
    public ParkingRow(bool hasOpposing, int rownum, int totalSpaces, string rowEnum, int opposingRow)
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
        if(position < _totalSpaces)
        {
            return parkingSpace[position];
        }
        throw new ArgumentOutOfRangeException();
    }
    string IParkingRow<IParkingSpace>.RowEnumeration => _rowEnumeration;
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
            space[i] = ISimpleFactory<>
                
                new ParkingSpace(string.Format("{0}:{1}",Row.ToString(),this._rowEnumeration), i);
        }
        return space;
    }

    bool IParkingSpace.IsTaken(int position)
    {

    }

    IParkingSpace IParkingRow<IParkingSpace>.GetParking(int position)
    {
        throw new NotImplementedException();
    }
    public interface ISimpleFactory<T, U> where T : IFactory<U>
    { 
        static T Factory { get; }
        static virtual U Create(params object[] parameters)
        {
            return Factory.Imp;
        }
    }
    public interface IFactory<T>
    {
        public object[] parameters { get; }
        public T Imp { get; }
    }
    public class RowFactory : IFactory<Func<object[], IParkingSpace>>
    {
        public object[] parameters => GetConfigValues()
        private Func<object[], IParkingSpace> _impl;
        public Func<object[], IParkingSpace> Imp => 
        
        public RowFactory(Func<object[], IParkingSpace> func,
            IConfig<IParkingSpace> config,
            IEnumerable<IParkingSpace> space)
        {
            
        }
        private object[] GetConfigValues(
            IConfig<IParkingSpace> config,
            IEnumerable<IParkingSpace> space)
        {

            if (config.GetValues(space).IsCompletedSuccessfully)
            {

            }

            return config.GetValues(space);
        }

        public interface IConfig<T>
        {
             //Assumes single line of PropertyDenoterPropertyNameValueStartValue
             public static readonly string PropertyDenoter = "%%%0x0000F";

             //Ends with X, where X is a integer denoting how many lines
             public static readonly string SectionDenoter = "///0x0000F";

             //Assumes inline with PropertyName immidiatetly after PropertyName
             public static readonly string ValueStart = "$$$=";            

             //Ends with X, where X is a integer denoting how many lines
             //Becomes the inline value of property
             //Data in collection is seperated by local EOL variable
             //each individual piece of data sits on line and will iterativly be sent to
             //the translate function registered for the type.
             public readonly static  string CollectionDenote = PropertyDenoter + "###";
             public T? GetValues();
             public static Dictionary<string, Func<string, T, T>> Parsers { get; }
             public static bool RegisterMemberParserMethod(
             string memberName,
             Func<string, T, T> parserMethod)
             {
                return  Parsers.TryAdd(memberName, parserMethod);
             }
        }
        /// <summary>
        /// Config output type needs to be a simple POCO type, nested types unsupported for now
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <typeparam name="U">Incoming type</typeparam>
        public abstract class Config<T> : IConfig<T> 
        {
            public Config(int cacheSize, T value)
            {
                Value = value;
            }
            public string[] UnParsed { get; set; }
            static List<string> Cache = new List<string>();
            T Value;

            public static Dictionary<string, Func<string, T, T>> Parsers { get; }
            /// <summary>
            /// Register Parsing method for member of type
            /// </summary>
            /// <param name="name">property name</param>
            /// <param name="Parse">value as string, returns T type, Sends in T ref to update</param>
            /// <returns></returns>
            public static bool RegisterMemberParserMethod(string name, Func<string, T, T> Parse)
            {
                return Parsers.TryAdd(name, Parse);
            }
            public T GetValues()
            {
                if (Cache.Count() == 0)
                {
                    var res = Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData,
                    Environment.SpecialFolderOption.Create);
                    SafeFileHandle handle = new();
                    ReadHandle(res);
                }
                return Value;
            }
            private T? CacheConverter()
            {
                int start = Array.FindIndex(Cache.ToArray(),
                    x => x.StartsWith(IConfig<T>.SectionDenoter)
                    && x.Equals(IConfig<T>.SectionDenoter + typeof(T).Name));

                int End = Array.FindIndex(Cache.ToArray(), start + 1,
                    x => x.StartsWith(IConfig<T>.SectionDenoter));
                int? collection = 0;
                int? collectionS;
                Func<string, T, T>? collectionParser;
                for (int i = start; i < End; i++)
                {
                    var res = FindParser(Cache[i], out collection);
                    if(collection >0)
                    {
                        collectionS = i;
                        collectionParser = res;
                        continue;
                    }
                    return res(RemoveDenoters(Cache[i], false), Value);
                }
                return Value;
            }
            //this should all just be a single RegeX, alas...
            private Func<string, T, T> FindParser(string line, out int? collection)
            {
                Func<string, T, T>? parser;
                string paramName = "";
                for (int k = 0; k < Parsers.Count(); k++)
                {
                    string registeredParsers = Parsers.Keys.ToList()[k];
                    var res = ParseDenoters(line,
                        registeredParsers,
                        out collection);
                    return res;
                }
                collection = 0;
                return null;
            }
            public Func<string, T, T>? ParseDenoters(string Denoter,
                string registeredParsers,
                out int? collectionLen)
            {
                if (Denoter.Contains(registeredParsers)
                && !Denoter.Contains(IConfig<T>.CollectionDenote))
                {
                    string parsername = Denoter.Remove(0,
                    IConfig<T>.PropertyDenoter.Length);
                    collectionLen = 0;
                    return  Parsers[parsername];
                }
                else if(Denoter.Contains(
                        registeredParsers
                    + IConfig<T>.ValueStart
                    + IConfig<T>.CollectionDenote))
                {
                    string result = Denoter.Remove(0,
                        registeredParsers.Length
                        + IConfig<T>.ValueStart.Length
                        + IConfig<T>.CollectionDenote.Length
                        + IConfig<T>.PropertyDenoter.Length);
                    collectionLen = GetCollectionLen(result);
                    return Parsers[registeredParsers];
                }
                collectionLen = 0;
                return null;
            }
            private int GetCollectionLen(string collection)
            {
                int res;
                string parsed = (collection[3].ToString() + collection[4].ToString());
                if(int.TryParse(parsed, out res))
                {
                    return res;
                }
                throw new Exception("CONFIG HAS BEEN EXTERNALLY MODIFIED");
            }
            private void ReadHandle(string path)
            {
                using (StreamReader reader = new(path))
                {
                    int i = 0;
                    while ((Cache[i] = reader.ReadLine()) is not null)
                    {
                        i++;
                    }
                }
            }
            private string RemoveDenoters(string val, bool collection)
            {
                if (collection)
                    return val;
                var res = val.IndexOf(IConfig<T>.ValueStart);
                return val.Remove(0, res+1);
            }
        }
    }
}
