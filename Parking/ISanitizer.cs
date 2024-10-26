public interface ISanitizer
{
    object Input { get; }
    string? Type { get; }
    //Overload Function to return the correct types
    object? Sanitize(out bool Result);
    void GCCollect();
}
