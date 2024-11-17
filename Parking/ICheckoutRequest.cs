public interface ICheckoutRequest
{
    string? ParkingHashStart { get; }
    IOwner? Owner { get; set; }
    IRecoverHash? RecoveryData { get; set; }
}
