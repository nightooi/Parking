using System.Runtime.InteropServices.ObjectiveC;

public interface IParkManagerEngine
{
    //This has to have a side effect of some sort of message or return a message
    // either a viewmodel object or something like that.

    //this returns just a plain ok screen
    IResultMessage HasSpace(VehicleSize size);
    //needs to show a status of what vehicles are allowed to come in.
    IResultMessage HasSpace();
    //shows that there is no space.
    IResultMessage NoSpace();
    IResultMessage RequestPark(object data);
    IResultMessage RequestCheckout(object data);
}
public interface IResultMessage
{
    string Type { get; }
    object Item { get; }
    object Subtype(string item);
}
public interface IResultGenerator
{
    IResultMessage GenerateResultMessage(object[] items);
}
