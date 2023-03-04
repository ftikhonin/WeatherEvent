namespace WeatherEventClient.Models;

public class AggregatedEvent
{
    public long Id { get; set; }
    
    /// <summary> Идентификатор сенсора </summary>
    public long SensorId { get; set; }
    
    /// <summary> Средняя температура </summary>
    public double AvgTemperature { get; set; }
    
    /// <summary> Средняя влажность </summary>
    public double AvgHumidity { get; set; }
    
    /// <summary> Макс содержание CO2 </summary>
    public long MaxCarbonDioxide { get; set; } 
    /// <summary> Мин содержание CO2 </summary>
    public long MinCarbonDioxide { get; set; } 
    
    public DateTime CreatedAt { get; set; }
}