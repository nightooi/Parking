using Parking.Config;
namespace Parking.Instantiations;

public class RowFactory : IFactory<IParkingSpace>
    {
        public object[] parameters => GetConfigValues();
        private Func<object[], IParkingSpace> _impl;
        Func<object[], IParkingSpace> _imp;
        IConfig<IParkingSpace> _config;
        public Func<object[], IParkingSpace> Imp => _imp;

        //Values from Config Are defined by the registered config returning type
        //as the first parameter, cast to that.
        public RowFactory(Func<object[], IParkingSpace> func,
            IConfig<IParkingSpace> config)
        {
            _imp = func;
            _config = config;
        }
        private object[] GetConfigValues()
        {
            return [_config.GetValues()];
        }
    }

