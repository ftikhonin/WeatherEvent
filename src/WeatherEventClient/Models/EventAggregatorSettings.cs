namespace WeatherEventClient.Models;

public class EventAggregatorSettings : IEventAggregatorSettings
{
    private int _aggregationPeriod = 1;

    public int AggregationPeriod
    {
        get => _aggregationPeriod;
        set
        {
            if (value >= 1)
            {
                _aggregationPeriod = value;
            }
        }
    }

    public int GetAggregationPeriod()
    {
        return AggregationPeriod;
    }

    public void SetAggregationPeriod(int aggregationPeriod)
    {
        AggregationPeriod = aggregationPeriod;
    }

    public List<long> SensorIDs = new();

    public void SetSensorIDs(List<long> ids)
    {
        SensorIDs.Clear();
        SensorIDs.AddRange(ids);
    }

    public List<long> GetSensorIDs()
    {
        return SensorIDs;
    }
}