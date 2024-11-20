
//
// Alternativly we can create a sort of ambient context
// in form of a RequestPark que with nessecary objects,
// this will clean up the constructor of this object
//
// Second approach is to create the que and have it be a ambient 
// context to a factory method returning Parked objects
//
using Parking.SingleTons;

public class Parked : IParked
{
    private IVehicle _vehicle;
    public IVehicle Vehicle => _vehicle;
    private string _parkingUId;
    private string registration;
    private IParkingTimer? parkingTimer { get; set; }
    public string ParkingUId => _parkingUId;
    public string Registration { get => registration; }
    public void StarParking()
    {
        if(parkingTimer is null)
        {
            this.parkingTimer = TimerFactorySingleton.Create();
        }
        this.parkingTimer.TimerStart();
    }
    //this must be a bug as far as im concerned.
    public TimeSpan EndParking()
    {
        if (parkingTimer is null) return new TimeSpan(0, 0, 0);
        return parkingTimer.Stop();
    }
    public TimeSpan ElapsedTime()
    {
       
        return this.parkingTimer.ElpsedTime().Duration();
    }
    public Parked(
        IParkingTimer timer,
        IParkingSpace space,
        string reg,
        IVehicle v)
    {
        this.parkingTimer = timer;
        this._parkingUId = space.UId;
        this.registration = reg;
        this._vehicle = v;
        this.PosPerType();
    }
    private string _pos;
    private string printPos => _pos;
    private void PosPerType()
    {
        if (this.Vehicle is Buss)
        {
            var res = UIDToPos.UIdToPos(this.ParkingUId);
            res.Item1++;
            _pos = new string([(char)res.Item1, (char)res.Item2]);
        }
    }
    public override string ToString()
    {
        
        return this.registration + " ::: "
            + this.ElapsedTime().Seconds + " ::: "
            + _pos + " :: " + Vehicle.GetType();
    }
}
public static class ParkedFactorySingleTon
{
    public static IParked Create(string reg, IParkingSpace space, IVehicle vehicle)
    {
        var timer = TimerFactorySingleton.Create();
        return new Parked(timer, space, reg, vehicle);
    }
}
