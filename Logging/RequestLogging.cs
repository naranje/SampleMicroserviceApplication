using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibOwin;
using Serilog;

namespace Logging
{
    using AppFunc = Func<IDictionary<string, object>, Task>;
    /// <summary>
    /// Middleware that logs requests and responses
    /// </summary>
    public class RequestLogging
    {
        public static AppFunc Middleware(AppFunc next, ILogger log)
        {
            return async env =>
            {
                var owinContext = new OwinContext(env);

                //Sends a log message with the request method, path, and headers
                log.Information("Incoming request: {@Method}, {@Path}, {@Headers}",
                    owinContext.Request.Method,
                    owinContext.Request.Path,
                    owinContext.Request.Headers);
                
                //Sends the request through the rest of the pipeline
                await next(env).ConfigureAwait(false);

                //Sends a log message with the response status code and headers
                log.Information("Outgoing response: {@StatusCode}, {@Headers}",
                    owinContext.Response.StatusCode,
                    owinContext.Response.Headers);
            };
        }
    }
}