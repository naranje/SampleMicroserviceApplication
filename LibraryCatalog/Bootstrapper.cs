using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using Platform;
using Serilog;

namespace LibraryCatalog
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        private readonly ILogger _logger;
        public Bootstrapper(ILogger logger)
        {
            _logger = logger;
        }
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            container.Register(_logger);
            container.UseHttpClientFactory(new NancyContext());
        }

        protected override void RequestStartup(
            TinyIoCContainer container,
            IPipelines pipelines,
            NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);
            container.UseHttpClientFactory(context);
        }
    }
}