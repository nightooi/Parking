public interface ISanitizer
{
    object? Input { get; }
    // this should be held in a const string array somewhere
    // And its values defined in a config
    // atleast until we can use generics
    string ExpectedType { get; }

    //
    //Overload Function to return the correct types?
    //-> this would bind the code to the implementation to much.
    // once again until we're a go on generics this unnessecary.
    //

    object? Sanitize(out bool Result);
    void GCCollect();
}
