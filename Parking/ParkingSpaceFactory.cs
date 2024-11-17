using Parking.Config;

using static ParkingRow;

namespace Parking.Instatiations;

/// <summary>
/// Needs to check wether the application shutdown correctly at some point;
/// </summary>
public class ParkingSpaceFactory : IFactory<IParkingSpace>
    {
        IConfig<IParkingSpace> Config;
        public object[] parameters => GetParamers();
        Func<object[], IParkingSpace> _impl;
        public Func<object[], IParkingSpace> Imp => _impl;
        private object[] GetParamers()
        {
            return [Config.GetValues()];
        }
    }
}
