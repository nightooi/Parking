public interface IPaymentType
{
    void Payment(float amount, IOwner owner);
    void Payment(float amount);
}
