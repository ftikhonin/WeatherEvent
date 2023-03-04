using WeatherEventGenerator.Extensions;
using WeatherEventGenerator.Models;

namespace WeatherEventGenerator
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSomeServices();
            services.Configure<SensorSettings>(_configuration.GetSection(nameof(SensorSettings)));
            services.AddControllers();
            services.AddGrpc();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapGrpcService<GrpcServices.WeatherEventStreamService>();
                }
            );
        }
    }
}