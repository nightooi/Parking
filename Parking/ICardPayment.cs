public interface ICardPayment : IPaymentType
{
    public enum TerminalResult { Denied, Approved, Fault }
    TerminalResult PaymentTerminal();
}
