using System.Security.Cryptography.X509Certificates;

public interface IParkingTimer
{
    void TimerStart();
    TimeSpan ElpsedTime();
    TimeSpan Stop();
}
