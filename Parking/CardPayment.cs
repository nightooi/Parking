public interface CardPayment : IPaymentType
{
    public enum TerminalResult { Denied, Approved, Fault }
    TerminalResult PaymentTerminal();
}
