using Microsoft.Extensions.Options;
using WeatherEventGenerator.BL;
using WeatherEventGenerator.Models;

namespace WeatherEventGenerator.HostedServices
{
    public class EventGeneratorService : BackgroundService
    {
        private readonly IEventStorage _storage;
        private readonly SensorSettings _sensorSettings;

        public EventGeneratorService(IEventStorage storage,
            IOptions<SensorSettings> sensorOptions)
        {
            _storage = storage;
            _sensorSettings = sensorOptions.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var i = 0;
            Random rand = new Random();
            var streetTemperature = rand.Next(-30, 31); //Температура на улице
            var homeTemperature = rand.Next(21, 27); //Температура в помещении
            var streetHumidity = rand.Next(0, 101); //Влажность 0-100
            var homeHumidity = rand.Next(0, 101); //Влажность 0-100
            var streetCarbonDioxide = rand.Next(350, 401); //CO2 на улице, 350-400 [ppm]
            var homeCarbonDioxide = rand.Next(400, 1000); //CO2 в помещении 400-1000 [ppm]

            //Создание сенсоров
            Dictionary<int, SensorType> sensors = new();
            for (int j = 1; j <= _sensorSettings.SensorCount; j++)
            {
                //Выберем тип датчика


                Type type = typeof(SensorType);
                Array values = type.GetEnumValues();
                var st = (SensorType)(values.GetValue(j % 2) ?? throw new InvalidOperationException());
                sensors.TryAdd(j, st);
            }

            //генерация событий
            while (!stoppingToken.IsCancellationRequested)
            {
                i++;
                var sensorId = sensors.ElementAt(rand.Next(0, sensors.Count)).Key;
                var sensorType = sensors[sensorId];

                //Если sensorId = 1, то считаем что это уличный датчик
                var temperature = Math.Round((rand.Next(0, 2) == 1 ? 1 : -1) * rand.NextDouble() / 10 +
                                             (sensorType == SensorType.Street
                                                 ? streetTemperature
                                                 : homeTemperature), 2);
                var carbonDioxide = Convert.ToInt32(
                    (rand.Next(0, 2) == 1 ? 1 : -1) *
                    (sensorType == SensorType.Street ? rand.NextDouble() : rand.Next(0, 11)) +
                    (sensorType == SensorType.Street ? streetCarbonDioxide : homeCarbonDioxide));
                var humidity =
                    Convert.ToInt32(
                        (rand.Next(0, 2) == 1 ? 1 : -1) *
                        (sensorType == SensorType.Street ? rand.NextDouble() : rand.Next(0, 11)) +
                        (sensorType == SensorType.Street ? streetHumidity : homeHumidity));
                _storage.AddEvent(new Event
                {
                    Id = i,
                    SensorType = sensorType,
                    SensorId = sensorId,
                    Temperature = temperature,
                    Humidity = humidity,
                    CarbonDioxide = carbonDioxide,
                    CreatedAt = DateTime.UtcNow,
                });
                Console.WriteLine(
                    "added with id={0} SensorID ={1} Temperature={2} Humidity={3} CarbonDioxide={4} SensorType ={5}",
                    i,
                    sensorId, temperature, humidity, carbonDioxide, sensorType);

                await Task.Delay(_sensorSettings.Interval, stoppingToken);
            }
        }
    }
}