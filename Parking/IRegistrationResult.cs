public interface IRegistrationResult : IRequestResult<ITimer>
{
    string? ParkingHash { get; }
    IParked? Parked { get; }
}
