using System.Collections.Concurrent;

namespace WeatherEventGenerator.BL;

public class EventStorage : IEventStorage
{
    private readonly ConcurrentDictionary<long, Event> _events = new();

    public void AddEvent(Event @event)
    {
        _events.AddOrUpdate(@event.Id, _ => @event, (_, _) => @event);
    }

    public void RemoveEvent(int id)
    {
        _events.Remove(id, out _);
    }

    public Event GetById(int id)
    {
        if (_events.TryGetValue(id, out var @event))
        {
            return @event;
        }

        throw new KeyNotFoundException("Event not found with ID=" + id);
    }

    public Event GetLastEvent()
    {
        return _events.Values.Last();
    }

    public Event GetLastEventBySensorId(int id)
    {
        Event result = new Event();
        for (int i = _events.Count; i > 0; i--)
        {
            var ev = GetById(i);
            if (ev.SensorId == id)
            {
                result = ev;
                break;
            }
        }

        return result;
    }

    public int Count()
    {
        return _events.Count;
    }

    public void Clear()
    {
        _events.Clear();
    }
}