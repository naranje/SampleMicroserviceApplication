using System.Threading.Tasks;
using Logging;
using Platform;
using Serilog;
using Serilog.Events;
using Microsoft.AspNetCore.Builder;
using Nancy.Owin;

namespace BookRequest
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            var logService = ConfigureLogger();
            //app.UseOwin(buildFunc =>
            //    {
            //        buildFunc(next => GlobalErrorLogging.Middleware(next, logService));
            //        buildFunc(next => CorrelationToken.Middleware(next));
            //        buildFunc(next => RequestLogging.Middleware(next, logService));
            //        buildFunc(next => PerformanceLogging.Middleware(next, logService));
            //        buildFunc(next => new MonitoringMiddleware(next, HealthCheck).Invoke);
            //        buildFunc.UseNancy(opt => opt.Bootstrapper = new Bootstrapper(logService));
            //    }
            //);
            //Extension method to add the monitoring and logging in the correct order
            app.UseOwin()
            .UseMonitoringAndLogging(logService, HealthCheck)
            .UseNancy(opt => opt.Bootstrapper = new Bootstrapper(logService));
        }

        private Task<bool> HealthCheck()
        {
            return Task.FromResult(true);
        }

        private ILogger ConfigureLogger()
        {
            MicroservicePlatform.Configure(
                tokenUrl: "http://localhost:5200/",
                clientName: "book_request",
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