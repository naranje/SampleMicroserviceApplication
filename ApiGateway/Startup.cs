using Logging;
using Platform;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Nancy.Owin;
using Serilog;
using Serilog.Events;

namespace ApiGateway
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            var logger = ConfigureLogger();
            app.UseStaticFiles();
            app.UseOwin()
                .UseMonitoringAndLogging(logger, HealthCheck)
                .UseNancy(opt => opt.Bootstrapper = new Bootstrapper(logger));
        }

        private ILogger ConfigureLogger()
        {
            MicroservicePlatform.Configure(
                tokenUrl: "http://localhost:5001/",
                clientName: "api_gateway",
                clientSecret: "secret");
            return new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.ColoredConsole(
                    LogEventLevel.Verbose,
                    "{NewLine}{Timestamp:HH:mm:ss} [{Level}] ({CorrelationToken}) {Message}{NewLine}{Exception}")
                .CreateLogger();
        }

        private static Task<bool> HealthCheck()
        {
            return Task.FromResult(true);
        }
    }
}