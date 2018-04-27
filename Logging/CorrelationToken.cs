using System;
using LibOwin;
using Serilog.Context;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

namespace Logging
{
    /// <summary>
    /// Middleware that sets a correlation token on the logging context for each request
    /// </summary>
    public class CorrelationToken
    {
        public static AppFunc Middleware(AppFunc next)
        {
            return async env =>
            {
                var owinContext = new OwinContext(env);

                //Tries to find a correlation token in a request header
                if (!(owinContext.Request.Headers["Correlation-Token"] != null
                      && Guid.TryParse(owinContext.Request.Headers["Correlation-Token"], out Guid correlationToken)))
                    correlationToken = Guid.NewGuid();

                //Add the correlation token to the OWIN environment it so we can retrieve
                //it when it needs to be attached to any requests to downstream 
                //microservices.
                owinContext.Set("correlationToken", correlationToken.ToString());

                //Adds the correlation token to the log context 
                //This just means it gets appended to all the logging messages 
                //The correlation token is in the context for all the logging from this point on
                //including rest of the pipeline
                using (LogContext.PushProperty("CorrelationToken", correlationToken))
                    await next(env).ConfigureAwait(false);
            };
        }
    }
}