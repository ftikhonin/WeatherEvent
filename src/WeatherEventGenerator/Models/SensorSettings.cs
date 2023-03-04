namespace WeatherEventGenerator.Models
{
    public class SensorSettings
    {
        private int _interval = 2000;
        private int _sensorCount = 0;

        public int Interval
        {
            get => _interval;
            set
            {
                if (value is >= 100 and <= 2000)
                {
                    _interval = value;
                }
            }
        }

        public int SensorCount
        {
            get => _sensorCount;
            set
            {
                if (value is > 0)
                {
                    _sensorCount = value;
                }
            }
        }
    }
}