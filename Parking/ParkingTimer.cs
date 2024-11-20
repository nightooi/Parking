public struct ParkingTimer : IParkingTimer
{
    private DateTime start;
    public TimeSpan ElpsedTime()
    {
        return DateTime.Now.Subtract(start);
    }
    public TimeSpan Stop()
    {
        return ElpsedTime();
    }
    public void TimerStart()
    {
        this.start = DateTime.Now;
    }
    public ParkingTimer()
    {
        this.start = DateTime.Now;
    }
}
