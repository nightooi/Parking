public interface IOwner
{
    string FirstName { get; }
    string LastName { get; }
    IVehicle[] Vehicle { get; }
    bool AddVehicle(IVehicle vehicle);
}
