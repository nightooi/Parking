namespace Parking.Instantiations;

public interface IFactory<T>
    {
        public object[] parameters { get; }
        public Func<object[], T> Imp { get; }
    }

