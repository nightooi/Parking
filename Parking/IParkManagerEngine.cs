using Parking.Instantiations;

using System.Runtime.InteropServices.ObjectiveC;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;

public interface IParkManagerEngine
{
    //This has to have a side effect of some sort of message or return a message
    // either a viewmodel object or something like that.
    //this returns just a plain ok screen

    IResultMessage HasSpace(VehicleSize size);
    //needs to show a status of what vehicles are allowed to come in.
    IResultMessage HasSpace();
    //shows that there is no space.
    IResultMessage RequestPark(IParkingRequest request);
    IResultMessage RequestCheckout(ICheckoutRequest request);
}
public class ParkManagerEngine : IParkManagerEngine
{
    ISimpleFactory<IResultGenerator, IResultMessage> mGenerator;
    IList<IResultMessage> inComingBuffer;
    IParkingLot ParkingLot;
    IDictionary<string, IParked> _parked;
    ISimpleFactory<IFactory<IPaymentTerminal>, IPaymentTerminal> paymentFactory;
    ISimpleFactory<IFactory<IParked>, IParked> parkedGen;
    public ParkManagerEngine(
        ISimpleFactory<IResultGenerator, IResultMessage> generator,
        ISimpleFactory<IFactory<IParkingLot>, IParkingLot> parkinglotFactory,
        ISimpleFactory<IFactory<IParked>, IParked> parkedFactory)
    {
        parkedGen = parkedFactory;
        mGenerator = generator;
        ParkingLot = parkinglotFactory.Create();
    }
    public IResultMessage HasSpace(VehicleSize size)
    {
        return mGenerator.Create([
            "HasSpace Type",
            $"{size.ToString()}",
            !ParkingLot.UnavailableFor.Contains(size)]) ;
    }
    public IResultMessage HasSpace()
    {
        return mGenerator.Create([
            "HasSpace Any",
            $"{VehicleSize.small}",
            !ParkingLot.UnavailableFor.Contains(VehicleSize.small),
            $"{VehicleSize.normal}",
            !ParkingLot.UnavailableFor.Contains(VehicleSize.normal),
            $"{VehicleSize.large}",
            !ParkingLot.UnavailableFor.Contains(VehicleSize.large),
            ]);
    }
    public IResultMessage RequestCheckout(ICheckoutRequest request)
    {
        IParked parked;
        IPaymentType payment;
        if (request.ParkingHashStart is not null)
        {
            if(_parked.TryGetValue(request.ParkingHashStart,out parked))
            {
                var parktime = parked.EndParking();
                this.ParkingLot.FreeSpot(parked.ParkingUId);
                _parked.Remove(request.ParkingHashStart);
                return mGenerator.Create(["EndParking", "TimeSpan", parktime]);
            }
            else
            {
                return mGenerator.Create("HashWrong");
            }
        }
        var start = request.RecoveryData.ParkStart;
        var name = CreateNameString(request.RecoveryData.Ownder);
        var res = this.HashData(request.RecoveryData.ParkStart, name, request.RecoveryData.Ownder.Vehicle[0]);
        _parked.TryGetValue(res.Substring(0, 5), out parked);
        this.ParkingLot.FreeSpot(parked.ParkingUId);
        return mGenerator.Create(["EndParking", "TimeSpan", parked.EndParking()]);
    }
    private string CreateNameString(IOwner owner)
    {
        return owner.FirstName + " " + owner.LastName;
    }
    /// <summary>
    /// Diffie hellman approximation
    /// </summary>
    /// <param name="Start">park start</param>
    /// <param name="name">name</param>
    /// <param name="vehicle">vehicle</param>
    /// <returns></returns>
    private string HashData(TimeOnly Start, string name, IVehicle vehicle)
    {
        string Data = name + vehicle.Number.RegNumber + vehicle.Color.Name;
        var charD = Data.ToCharArray();
        byte[] var = [(byte)(Start.Hour), (byte)(Start.Hour % 2), (byte)(Start.Hour % 3)];
        int[] blend = new int[charD.Length + var.Length];
        int k = 0;
        string res = "";
        for(int i =0; i < blend.Length; i++)
        {
            if(i> var.Length)
            {
                blend[i] = var[i] % var[k];
                continue;
            }
            blend[i] = (byte)charD[i] % var[k];
            k++;
            k = (k > 3) ? 0 : k;
        }
        for(int i= 0; i <blend.Length; i++)
        {
            res += (char)blend[i];
        }
        return res;
    }
    public IResultMessage RequestPark(IParkingRequest request)
    {
        var space = this.ParkingLot.AssignSpot(request.Owner.Vehicle[0]);
        var res = parkedGen.Create([request.Owner, space]);
        res.StarParking();
        var start = DateTime.Now;
        var time = new TimeOnly(start.Hour, start.Minute);
        var hash = this.HashData(time, CreateNameString(request.Owner), request.Owner.Vehicle[0]);
        return mGenerator.Create("StartPark", "hash", hash);
    }
}

public interface IRecoverHash
{
    TimeOnly ParkStart { get; set; }
    IOwner Ownder { get; set; }
}

public class RecoverHash : IRecoverHash
{
    public TimeOnly ParkStart { get; set; }
    public IOwner Ownder { get; set; }

    RecoverHash(TimeOnly start, IOwner owner)
    {
        Ownder = owner;
    }
}
public interface IResultMessage
{
    string Type { get; }
    object Item { get; }
    object Subtype(string item);
}
public abstract class  Message : IResultMessage
{
    string _messageType;
    public string Type => _messageType;

    object _resultObject;
    public object Item => _resultObject;

    public object Subtype(string item)
    {
        throw new NotImplementedException();
    }
}

public class RegistrationMessage : Message
{

}
public class EndParkingMessage : Message
{

}
public interface IResultGenerator : IFactory<IResultMessage>
{
}

public class ResultGenerator : IResultGenerator
{
    public object[] parameters => throw new NotImplementedException();

    public Func<object[], IResultMessage> Imp => throw new NotImplementedException();


}
