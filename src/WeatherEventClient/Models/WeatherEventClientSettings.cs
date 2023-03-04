namespace WeatherEventClient.Models;

public class WeatherEventClientSettings
{
    private int _waitTime = 100;

    public int WaitTime
    {
        get => _waitTime;
        set
        {
            if (value is > 0)
            {
                _waitTime = value;
            }
        }
    }
}