using WeatherEventGenerator.BL;
using WeatherEventGenerator.HostedServices;

namespace WeatherEventGenerator.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSomeServices(this IServiceCollection services)
        {
            services.AddHostedService<EventGeneratorService>();
            services.AddSingleton<IEventStorage, EventStorage>();
            return services;
        }
    }
}