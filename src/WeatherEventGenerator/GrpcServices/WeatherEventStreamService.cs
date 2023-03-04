using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using WeatherEventGenerator.BL;

namespace WeatherEventGenerator.GrpcServices;

public class WeatherEventStreamService : WeatherEventGenerator.WeatherEventStreamService.WeatherEventStreamServiceBase
{
    private readonly IEventStorage _eventStore;

    public WeatherEventStreamService(IEventStorage eventStore)
    {
        _eventStore = eventStore;
    }

    public override async Task WeatherEventStream(Empty request,
        IServerStreamWriter<EventStreamResponse> responseStream, ServerCallContext context)
    {
        var rand = new Random();
        while (!context.CancellationToken.IsCancellationRequested)
        {
            await Task.Delay(rand.Next(100, 500), context.CancellationToken);
            var ev = _eventStore.GetLastEvent();
            var result = new EventStreamResponse()
            {
                CarbonDioxide = ev.CarbonDioxide,
                CreatedAt = ev.CreatedAt.ToTimestamp(),
                Humidity = ev.Humidity,
                Id = ev.Id,
                SensorId = ev.SensorId,
                SensorType = ev.SensorType == SensorType.Home? StreamSensorType.Home : StreamSensorType.Street,
                Temperature = ev.Temperature
            };
            await responseStream.WriteAsync(result, context.CancellationToken);
        }
    }
    
    public override Task WeatherEventStreamDuplex(
        IAsyncStreamReader<Empty> requestStream,
        IServerStreamWriter<EventStreamResponse> responseStream,
        ServerCallContext context)
    {
        return base.WeatherEventStreamDuplex(requestStream, responseStream, context);
    }
}