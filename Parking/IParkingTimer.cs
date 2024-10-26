public interface IParkingTimer
{
    Timer Timer { get; }
    Timer TimerStart();
    Timer ElpsedTime();
}
