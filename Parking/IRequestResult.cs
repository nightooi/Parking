public interface IRequestResult<T>
{
    Fault? FailedRequest { get; }
    bool Succeeded(out T? result);
}
