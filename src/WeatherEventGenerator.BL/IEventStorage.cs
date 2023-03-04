namespace WeatherEventGenerator.BL;

public interface IEventStorage
{
    public void AddEvent(Event @event);

    public void RemoveEvent(int id);

    public Event GetById(int id);

    public Event GetLastEventBySensorId(int id);
    public int Count();
    public Event GetLastEvent();
    public void Clear();
}