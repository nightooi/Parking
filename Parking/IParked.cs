public interface IParked
{
    IOwner Owner { get; set; }
    IRegistrationNumber Registration { get; set; }
    ITimer Timer { get; }
    void StarParking();
    IParked StartParking();
    ITimer EndParking();
}
