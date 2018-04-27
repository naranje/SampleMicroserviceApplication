using System;
using System.Threading.Tasks;
using Serilog;

namespace Logging
{
    //Owin BuildFunc
    //When UseOwin is called you can pass it in an Action that inserts the
    //middleware into the pipeline.  
    using BuildFunc = Action<Func<
    Func<System.Collections.Generic.IDictionary<string, object>, Task>,
    Func<System.Collections.Generic.IDictionary<string, object>, Task>>>;

  public static class BuildFuncExtensions
  {
    public static BuildFunc UseMonitoringAndLogging(
      this BuildFunc buildFunc,
      ILogger log,
      Func<Task<bool>> healthCheck)
    {

      buildFunc(next => GlobalErrorLogging.Middleware(next, log));
      buildFunc(next => CorrelationToken.Middleware(next));
      buildFunc(next => RequestLogging.Middleware(next, log));
      buildFunc(next => PerformanceLogging.Middleware(next, log));
      buildFunc(next => new MonitoringMiddleware(next, healthCheck).Invoke);
        //returns BuildFunc to allow chaining of calls to BuildFunc extensions
        return buildFunc;
    }
  }
}