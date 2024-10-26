public interface IParkingRequest
{
    IRegistrationResult RequestParkingSpace(IOwner owner, IVehicle vehicle);
    IRegistrationResult RequestExtension(IOwner owner, IVehicle vehicle);
}
