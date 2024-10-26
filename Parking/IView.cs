public interface IView
{
    IBoundProperty Bind(object item);
    void Update();
}

public interface IBoundProperty
{
    object ViewProperty { get; }
    string Item { get; }
    bool SetOutput();
    bool Update();
}
