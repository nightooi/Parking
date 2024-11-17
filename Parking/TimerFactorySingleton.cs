namespace Parking.SingleTons;

public static class TimerFactorySingleton
{
    public static IParkingTimer Create()
    {
        return new ParkingTimer();
    }
}
