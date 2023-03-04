using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;
using WeatherEventClient.Models;
using WeatherEventGenerator;

namespace WeatherEventClient.HostedServices
{
    public class EventAggregatorService : BackgroundService
    {
        private readonly IAggregatedEventStorage _storage;
        private readonly IServiceProvider _provider;
        private readonly IEventAggregatorSettings _settings;
        private readonly WeatherEventClientSettings _clientSettings;
        private int DeltaSeconds;
        private DateTime NowDateTime = DateTime.Now;

        /// <summary>
        /// Сервис агрегирует данные по заданному периоду в _settings.GetAggregationPeriod и по заданным датчикам из _settings.GetAggregationPeriod
        /// </summary>
        public EventAggregatorService(IAggregatedEventStorage storage, IServiceProvider provider,
            IEventAggregatorSettings eventAggregatorSettings, IOptions<WeatherEventClientSettings> clientSettings
        )
        {
            _storage = storage;
            _provider = provider;
            _settings = eventAggregatorSettings;
            _clientSettings = clientSettings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            long id = 1;
            await using var scope = _provider.CreateAsyncScope();
            while (true)
            {
                var client = scope.ServiceProvider
                    .GetRequiredService<WeatherEventStreamService.WeatherEventStreamServiceClient>();
                using var eventResponseStream =
                    client.WeatherEventStream(new Empty(), cancellationToken: stoppingToken);
                try
                {
                    List<EventStreamResponse> nonAggregated = new List<EventStreamResponse>();

                    while (await eventResponseStream.ResponseStream.MoveNext(stoppingToken))
                    {
                        var e = eventResponseStream.ResponseStream.Current;

                        if (_settings.GetSensorIDs().Contains(e.SensorId))
                        {
                            nonAggregated.Add(e);
                        }

                        DeltaSeconds = DateTime.Now.Subtract(NowDateTime).Seconds;
                        if (DeltaSeconds >= _settings.GetAggregationPeriod())
                        {
                            int i = 0;

                            if (nonAggregated.Count > 0)
                            {
                                foreach (var sensorID in _settings.GetSensorIDs())
                                {
                                    var aggregatedEvent = new AggregatedEvent();
                                    double humiditySum = 0;
                                    double temperatureSum = 0;
                                    long maxCarbonDioxide = 0;
                                    long minCarbonDioxide = long.MaxValue;
                                    bool isAdded = false;
                                    foreach (var row in nonAggregated)
                                    {
                                        if (row.SensorId == sensorID)
                                        {
                                            isAdded = true;
                                            maxCarbonDioxide = row.CarbonDioxide > maxCarbonDioxide
                                                ? row.CarbonDioxide
                                                : maxCarbonDioxide;
                                            minCarbonDioxide = row.CarbonDioxide < minCarbonDioxide
                                                ? row.CarbonDioxide
                                                : minCarbonDioxide;
                                            i++;
                                            humiditySum += row.Humidity;
                                            temperatureSum += row.Temperature;
                                        }
                                    }

                                    if (isAdded)
                                    {
                                        aggregatedEvent.Id = id;
                                        aggregatedEvent.SensorId = sensorID;
                                        aggregatedEvent.AvgHumidity = humiditySum / i;
                                        aggregatedEvent.AvgTemperature = temperatureSum / i;
                                        aggregatedEvent.CreatedAt = DateTime.Now;
                                        aggregatedEvent.MaxCarbonDioxide = maxCarbonDioxide;
                                        aggregatedEvent.MinCarbonDioxide = minCarbonDioxide;
                                        
                                        Console.WriteLine(String.Concat(
                                            "added with SensorID =", aggregatedEvent.SensorId, " AvgHumidity=",
                                            aggregatedEvent.AvgHumidity, " AvgTemperature=",
                                            aggregatedEvent.AvgTemperature,
                                            " MaxCarbonDioxide=", aggregatedEvent.MaxCarbonDioxide,
                                            " MinCarbonDioxide =", aggregatedEvent.MinCarbonDioxide, " Date=",
                                            DateTime.Now.ToString(), " DeltaSeconds=", DeltaSeconds.ToString(),
                                            " aggregationPeriod=", _settings.GetAggregationPeriod(), " cnt=", i,
                                            " Datetime=", DateTime.Now.ToString()
                                        ));
                                        _storage.AddEvent(aggregatedEvent);
                                        id++;
                                    }
                                }
                            }

                            DeltaSeconds = 0;
                            NowDateTime = DateTime.Now;
                            nonAggregated.Clear();
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Getting Exception : {e.Message}");
                    await Task.Delay(_clientSettings.WaitTime);
                }
            }
        }
    }
}