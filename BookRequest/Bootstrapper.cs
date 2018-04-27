using BookRequest.LibraryCatalog;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Configuration;
using Nancy.TinyIoc;
using Platform;
using Serilog;

namespace BookRequest
{
    /// <summary>
    /// Nancy Bootstrapper is used for Nancy Module discovery and framework composition at runtime
    /// By default its built on top of the TinyIOC container
    /// </summary>
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        private readonly ILogger _logger;
        public Bootstrapper(ILogger logger)
        {
            _logger = logger;
        }

        public override void Configure(INancyEnvironment env)
        {
            env.Tracing(enabled: true, displayErrorTraces: true);            
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
            container.Register<ILibraryCatalogClient>(new LibraryCatalogClient(container.Resolve<ICache>(), container.Resolve<IHttpClientFactory>()));
        }
    }
}