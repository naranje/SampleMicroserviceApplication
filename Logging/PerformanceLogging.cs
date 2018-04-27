using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using LibOwin;
using Serilog;

namespace Logging
{
    using AppFunc = Func<IDictionary<string, object>, Task>;
    /// <summary>
    /// Middleware that logs request times for all requests
    /// </summary>
    public class PerformanceLogging
    {
        public static AppFunc Middleware(AppFunc next, ILogger log)
        {
            return async env =>
            {
                var stopWatch = new Stopwatch();

                //Start the watch before executing the rest of the pipeline
                stopWatch.Start();

                await next(env).ConfigureAwait(false);
                
                //Stop the watch after execution
                stopWatch.Stop();

                //Send a log message with information about the request with the execution time
                var owinContext = new OwinContext(env);
                log.Information("Request: {@Method} {@Path} executed in {RequestTime:000} ms",
                    owinContext.Request.Method, owinContext.Request.Path,
                    stopWatch.ElapsedMilliseconds);
            };
        }
    }
}