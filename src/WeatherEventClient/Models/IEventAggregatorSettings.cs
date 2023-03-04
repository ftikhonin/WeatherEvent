namespace WeatherEventClient.Models;

public interface IEventAggregatorSettings
{
    public void SetSensorIDs(List<long> ids);
    public List<long> GetSensorIDs();
    public int GetAggregationPeriod();
    public void SetAggregationPeriod(int aggregationPeriod);
}