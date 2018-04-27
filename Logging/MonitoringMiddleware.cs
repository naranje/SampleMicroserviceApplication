using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibOwin;

namespace Logging
{
    //Owin AppFun signature
    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

    public class MonitoringMiddleware
    {
        private readonly AppFunc _next;
        private readonly Func<Task<bool>> _healthCheck;

        private static readonly PathString MonitorPath = new PathString("/_monitor");
        private static readonly PathString MonitorShallowPath = new PathString("/_monitor/shallow");
        private static readonly PathString MonitorDeepPath = new PathString("/_monitor/deep");

        public MonitoringMiddleware(AppFunc next, Func<Task<bool>> healthCheck)
        {
            _next = next;
            _healthCheck = healthCheck;
        }

        //Monitoring middleware AppFunc implementation that can be added to an OWIN pipeline
        public Task Invoke(IDictionary<string, object> env)
        {
            var context = new OwinContext(env);
            //If the incoming requiest is for a monitoring endpoint
            return context.Request.Path.StartsWithSegments(MonitorPath) ? HandleMonitorEndpoint(context) : _next(env);
        }

        private Task HandleMonitorEndpoint(OwinContext context)
        {
            if (context.Request.Path.StartsWithSegments(MonitorShallowPath))
                return ShallowEndpoint(context);

            return context.Request.Path.StartsWithSegments(MonitorDeepPath)
                ? DeepEndpoint(context)
                : Task.FromResult(0);
        }

        private async Task DeepEndpoint(OwinContext context)
        {
            //Each microservice implements its own deep health check function
            if (await _healthCheck().ConfigureAwait(false))
                context.Response.StatusCode = 204;
            else
                context.Response.StatusCode = 503;
        }

        private Task ShallowEndpoint(OwinContext context)
        {
            //Shallow check just responds with success
            context.Response.StatusCode = 204;
            return Task.FromResult(0);
        }
    }
}