namespace Parking.Instantiations;
//should be static but we have no DI container to its gonna have to be a implementation bound type
public interface ISimpleFactory<T, U> where T : IFactory<U>
{
    public T Factory { get; }
    public U Create()
    {
        return Factory.Imp(Factory.parameters);
    }
    public U Create(params object[] parameters)
    {
        return Factory.Imp(new[] { parameters, Factory.parameters });
    }
}
