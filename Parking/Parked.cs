
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
    private string _parkingUId;
    private IOwner _owner;
    private IRegistrationNumber registration;
    private IParkingTimer? parkingTimer { get; set; }
    public string ParkingUId => _parkingUId;
    public IOwner Owner { get => _owner; }
    public IRegistrationNumber Registration { get => registration; }
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
    public Parked(
        IParkingTimer timer,
        IParkingSpace space,
        IRegistrationNumber reg,
        IOwner owner)
    {
        this.parkingTimer = timer;
        this._parkingUId = space.UId;
        this.registration = reg;
        this._owner = owner;
    }
}
