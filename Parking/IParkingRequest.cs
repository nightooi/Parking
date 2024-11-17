public interface IParkingRequest
{
    IOwner Owner { get; set; }
    IRegistrationResult RequestParkingSpace(IOwner owner, IVehicle vehicle);
    IRegistrationResult RequestExtension(IOwner owner, IVehicle vehicle);
}
