using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using WeatherEventAggregator;
using WeatherEventClient.Models;
using AverageTemperatureResponse = WeatherEventClient.Models;
using SetResponse = WeatherEventAggregator.SetResponse;

namespace WeatherEventClient.GrpcServices;

public class WeatherEventAggregatorService : WeatherEventAggregator.WeatherEventAggregator.WeatherEventAggregatorBase
{
    private readonly IEventAggregatorSettings _settings;
    private readonly IAggregatedEventStorage _repository;

    public WeatherEventAggregatorService(IEventAggregatorSettings settings, IAggregatedEventStorage repository)
    {
        _settings = settings;
        _repository = repository;
    }
    /// <summary>
    /// Хэндлер задаёт период агрегации в BackgroundService
    /// </summary>
    public override Task<SetResponse> SetAggregationPeriod(AggregationPeriodRequest request,
        ServerCallContext context)
    {
        _settings.SetAggregationPeriod(request.AggregationPeriod);
        return Task.FromResult(new SetResponse { Msg = "Success" });
    }
    /// <summary>
    /// Хэндлер задаёт датчики, по которым будем агрегировать данные в BackgroundService
    /// </summary>
    public override Task<SetResponse> SetSensorIds(SensorRequest request,
        ServerCallContext context)
    {
        var sensorIds = new List<long>();

        foreach (var row in request.SensorIds)
        {
            sensorIds.Add(row);
        }

        _settings.SetSensorIDs(sensorIds);
        return Task.FromResult(new SetResponse { Msg = "Success" });
    }
    /// <summary>
    /// Хэнлдер возвращает среднюю температуру от заданной даты DateFrom и за указанное количество минут
    /// </summary>
    public override Task<WeatherEventAggregator.AverageTemperatureResponse> AverageTemperature(
        AggregationRequest request,
        ServerCallContext context)
    {
        var result = _repository.GetAverageTemperature(request.DateFrom.ToDateTime(), request.Minutes);
        
        return Task.FromResult(result);
    }
    /// <summary>
    /// Хэнлдер возвращает среднюю влажность от заданной даты DateFrom и за указанное количество минут
    /// </summary>
    public override Task<WeatherEventAggregator.AverageHumidityResponse> AverageHumidity(
        AggregationRequest request,
        ServerCallContext context)
    {
        var result = _repository.GetAverageHumidity(request.DateFrom.ToDateTime(), request.Minutes);
        
        return Task.FromResult(result);
    }
    /// <summary>
    /// Хэнлдер возвращает максимальное и минимальное содержание CO2 от заданной даты DateFrom и за указанное количество минут
    /// </summary>
    public override Task<WeatherEventAggregator.MaxMinCarbonDioxideResponse> MaxMinCarbonDioxide(
        AggregationRequest request,
        ServerCallContext context)
    {
        var result = _repository.GetMaxMinCarbonDioxide(request.DateFrom.ToDateTime(), request.Minutes);
        
        return Task.FromResult(result);
    }
    /// <summary>
    /// Хэнлдер выводит все сохранённые сагрегированные данные
    /// </summary>
    public override Task<WeatherEventAggregator.DiagnosticHandlerResponse> DiagnosticHandler(
        Empty request,
        ServerCallContext context)
    {
        var result = _repository.GetDiagnostic();
        
        return Task.FromResult(result);
    }
}