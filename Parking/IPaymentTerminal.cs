public interface IPaymentTerminal
{

    IPaymentType Pay(ITimer time, float price);
    
}
