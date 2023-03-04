using System.Collections.Concurrent;
using Google.Protobuf.WellKnownTypes;
using WeatherEventAggregator;
using WeatherEventClient.Models;
using AverageTemperatureResponse = WeatherEventAggregator.AverageTemperatureResponse;

namespace WeatherEventClient;

public class AggregatedEventStorage : IAggregatedEventStorage
{
    private readonly ConcurrentBag<AggregatedEvent> _events = new();

    public void AddEvent(AggregatedEvent @event)
    {
        _events.Add(@event);
    }

    public AverageTemperatureResponse GetAverageTemperature(DateTime dateFrom, int minutes)
    {
        var result = new AverageTemperatureResponse();
        var sum = new Dictionary<long, double>();
        var count = new Dictionary<long, int>();
        foreach (var row in _events)
        {
            long sensorID = row.SensorId;
            double avgTemperature;
            int counter;
            if (row.CreatedAt >= dateFrom && row.CreatedAt < dateFrom.AddMinutes(minutes))
            {
                sum.TryGetValue(sensorID, out avgTemperature);
                sum[sensorID] = avgTemperature + row.AvgTemperature;

                count.TryGetValue(sensorID, out counter);
                count[sensorID] = counter + 1;
            }
        }

        //агрегируем данные 
        foreach (var row in sum)
        {
            double sumBySensor;
            int cnt;
            sum.TryGetValue(row.Key, out sumBySensor);
            count.TryGetValue(row.Key, out cnt);
            result.Results.Add(new AverageTemperatureResponse.Types.AverageTemperature
            {
                AvgTemperature = sumBySensor / cnt, SensorId = row.Key
            });
        }

        return new AverageTemperatureResponse(result);
    }

    public AverageHumidityResponse GetAverageHumidity(DateTime dateFrom, int minutes)
    {
        var result = new AverageHumidityResponse();
        var sum = new Dictionary<long, double>();
        var count = new Dictionary<long, int>();
        foreach (var row in _events)
        {
            long sensorID = row.SensorId;
            double avgHumidity;
            int counter;
            if (row.CreatedAt >= dateFrom && row.CreatedAt < dateFrom.AddMinutes(minutes))
            {
                sum.TryGetValue(sensorID, out avgHumidity);
                sum[sensorID] = avgHumidity + row.AvgHumidity;

                count.TryGetValue(sensorID, out counter);
                count[sensorID] = counter + 1;
            }
        }

        //агрегируем данные 
        foreach (var row in sum)
        {
            double sumBySensor;
            int cnt;
            sum.TryGetValue(row.Key, out sumBySensor);
            count.TryGetValue(row.Key, out cnt);
            result.Results.Add(new AverageHumidityResponse.Types.AverageHumidity
            {
                AvgHumidity = sumBySensor / cnt, SensorId = row.Key
            });
        }

        return new AverageHumidityResponse(result);
    }

    public MaxMinCarbonDioxideResponse GetMaxMinCarbonDioxide(DateTime dateFrom, int minutes)
    {
        var result = new MaxMinCarbonDioxideResponse();
        var min = new Dictionary<long, long>();
        var max = new Dictionary<long, long>();
        foreach (var row in _events)
        {
            long sensorID = row.SensorId;
            min[sensorID] = row.MinCarbonDioxide;
            if (row.CreatedAt >= dateFrom && row.CreatedAt < dateFrom.AddMinutes(minutes))
            {
                long curMax;
                max.TryGetValue(sensorID, out curMax);
                max[sensorID] = curMax < row.MaxCarbonDioxide ? row.MaxCarbonDioxide : curMax;

                long curMin;
                min.TryGetValue(sensorID, out curMin);
                min[sensorID] = curMin > row.MinCarbonDioxide ? row.MinCarbonDioxide : curMin;
            }
        }

        //агрегируем данные 
        foreach (var row in min)
        {
            long minAggregated;
            long maxAggregated;
            min.TryGetValue(row.Key, out minAggregated);
            max.TryGetValue(row.Key, out maxAggregated);
            result.Results.Add(new MaxMinCarbonDioxideResponse.Types.MaxMinCarbonDioxide
            {
                MaxCarbonDioxide = maxAggregated, MinCarbonDioxide = minAggregated, SensorId = row.Key
            });
        }

        return new MaxMinCarbonDioxideResponse(result);
    }

    public DiagnosticHandlerResponse GetDiagnostic()
    {
        var result = new DiagnosticHandlerResponse();

        foreach (var row in _events)
        {
            result.Results.Add(new DiagnosticHandlerResponse.Types.DiagnosticHandler()
            {
                Id = row.Id,
                SensorId = row.SensorId,
                AvgHumidity = row.AvgHumidity,
                AvgTemperature = row.AvgTemperature,
                CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(row.CreatedAt, DateTimeKind.Utc)),
                MaxCarbonDioxide = row.MaxCarbonDioxide,
                MinCarbonDioxide = row.MinCarbonDioxide
            });
        }


        return new DiagnosticHandlerResponse(result);
    }
}