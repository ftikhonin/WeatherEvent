using WeatherEventAggregator;
using WeatherEventClient.Models;
using AverageTemperatureResponse = WeatherEventAggregator.AverageTemperatureResponse;

namespace WeatherEventClient;

public interface IAggregatedEventStorage
{
    public void AddEvent(AggregatedEvent @event);
    public AverageTemperatureResponse GetAverageTemperature(DateTime dateFrom, int minutes);
    public AverageHumidityResponse GetAverageHumidity(DateTime dateFrom, int minutes);
    public MaxMinCarbonDioxideResponse GetMaxMinCarbonDioxide(DateTime dateFrom, int minutes);
    public DiagnosticHandlerResponse GetDiagnostic();

}