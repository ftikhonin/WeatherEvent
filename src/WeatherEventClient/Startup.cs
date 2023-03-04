using WeatherEventClient.HostedServices;
using WeatherEventClient.Models;
using WeatherEventGenerator.GrpcServices;

namespace WeatherEventClient
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration
        )
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.Configure<WeatherEventClientSettings>(
                    _configuration.GetSection(nameof(WeatherEventClientSettings)));
                services.AddSingleton<IEventAggregatorSettings, EventAggregatorSettings>();
                services.AddSingleton<IAggregatedEventStorage, AggregatedEventStorage>();
                services.AddHostedService<EventAggregatorService>();
                services.AddGrpcClient<WeatherEventGenerator.WeatherEventStreamService.WeatherEventStreamServiceClient>(
                    options => { options.Address = new Uri("https://localhost:5002/"); });
                services.AddGrpc();
                services.AddControllers();
            }
            catch (Exception e)
            {
                Console.WriteLine("e.InnerException=");
                Console.WriteLine(e.InnerException);
                throw;
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapGrpcService<GrpcServices.WeatherEventAggregatorService>();
                    endpoints.MapGrpcService<WeatherEventStreamService>();
                }
            );
        }
    }
}