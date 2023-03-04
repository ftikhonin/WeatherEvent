using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WeatherEventGenerator.Models;
using WeatherEventGenerator.BL;

namespace WeatherEventGenerator.Controllers;

[Route("events")]
public class EventController : Controller
{
    private readonly IEventStorage _eventStore;
    private readonly SensorSettings _sensorSettings;

    public EventController(IEventStorage eventStore, IOptions<SensorSettings> sensorOptions)
    {
        _eventStore = eventStore;
        _sensorSettings = sensorOptions.Value;
    }

    [HttpGet]
    public Task<ActionResult<List<EventResponse>>> GetEvents()
    {
        var result = new List<EventResponse>();
        for (var i = 1; i <= _sensorSettings.SensorCount; i++)
        {
            var ev = GetLastEvents(i);
            var eventResponse = new EventResponse()
            {
                CarbonDioxide = ev.CarbonDioxide,
                CreatedAt = ev.CreatedAt,
                Humidity = ev.Humidity,
                Id = ev.Id,
                SensorType = ev.SensorType,
                SensorId = ev.SensorId,
                Temperature = ev.Temperature
            };
            result.Add(eventResponse);
        }

        return Task.FromResult<ActionResult<List<EventResponse>>>(Ok(result));
    }

    [HttpGet("{id:int}")]
    public Task<ActionResult<Event>> GetEvent(int id)
    {
        var ev = GetLastEvents(id);
        return Task.FromResult<ActionResult<Event>>(Ok(ev));
    }

    public Event GetLastEvents(int sensorId)
    {
        for (var i = _eventStore.Count(); i >= 0; i--)
        {
            var ev = _eventStore.GetLastEventBySensorId(sensorId);
            if (ev.SensorId == sensorId)
            {
                return ev;
            }
        }

        return new Event();
    }
}