using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using Platform;
using Serilog;

namespace ApiGateway
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

        /// <summary>
        /// Called by Nancy for each request
        /// </summary>
        /// <param name="container"></param>
        /// <param name="pipelines"></param>
        /// <param name="context"></param>
        protected override void RequestStartup(
            TinyIoCContainer container,
            IPipelines pipelines,
            NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);
            //Registers HttpClientFactory in Nancy's container
            container.UseHttpClientFactory(context);
        }
    }
}