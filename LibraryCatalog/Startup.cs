using System.Threading.Tasks;
using Logging;
using Platform;
using Serilog;
using Serilog.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Nancy.Owin;

namespace LibraryCatalog
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var logger = ConfigureLogger();
            app.UseOwin()
                .UseMonitoringAndLogging(logger, HealthCheck)
                .UseNancy(opt => opt.Bootstrapper = new Bootstrapper(logger));
        }

        private Task<bool> HealthCheck()
        {
            return Task.FromResult(true);
        }

        private Serilog.ILogger ConfigureLogger()
        {
            MicroservicePlatform.Configure(
                tokenUrl: "http://localhost:5100/",
                clientName: "library_catalog",
                clientSecret: "secret");
            return new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(
                    LogEventLevel.Verbose,
                    "{NewLine}{Timestamp:HH:mm:ss} [{Level}] ({CorrelationToken}) {Message}{NewLine}{Exception}")
                .CreateLogger();
        }
    }
}