namespace Parking.Config;
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
