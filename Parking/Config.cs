using Microsoft.Win32.SafeHandles;

public partial class ParkingRow
{
    public partial class RowFactory
    {
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

            /// <summary>
            /// Register Parsing method for member of type
            /// </summary>
            /// <param name="name">property name</param>
            /// <param name="Parse">value as string, returns T type, Sends in T ref to update</param>
            /// <returns></returns>
            public static bool RegisterMemberParserMethod(string name, Func<string, T, T> Parse)
            {
                return IConfig<T>.Parsers.TryAdd(name, Parse);
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
