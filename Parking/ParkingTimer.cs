public struct ParkingTimer : IParkingTimer
{
    private DateTime start;
    public TimeSpan ElpsedTime()
    {
        return DateTime.Now - start;
    }
    public TimeSpan Stop()
    {
        start = new DateTime();
        return ElpsedTime();
    }
    public void TimerStart()
    {
        start = DateTime.Now;
    }
    public ParkingTimer()
    {
        start = new DateTime();
    }
}
