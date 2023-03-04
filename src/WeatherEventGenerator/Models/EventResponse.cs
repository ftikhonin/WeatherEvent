using WeatherEventGenerator.BL;

namespace WeatherEventGenerator.Models;

public class EventResponse
{
    public long Id { get; set; }

    /// <summary> Идентификатор сенсора </summary>
    public long SensorId { get; set; }

    public SensorType SensorType { get; set; }

    /// <summary> Температура </summary>
    public double Temperature { get; set; }

    /// <summary> Влажность </summary>
    public long Humidity { get; set; }

    /// <summary> Содержание CO2 </summary>
    public long CarbonDioxide { get; set; }

    public DateTime CreatedAt { get; set; }
}