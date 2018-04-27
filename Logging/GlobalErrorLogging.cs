using System;
using Serilog;

namespace Logging
{
    //OWIN AppFunc signature
    using AppFunc = Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

    /// <summary>
    /// Middleware placed right at the start of the OWIN pipeline and catches and logs all unhandled exceptions
    /// </summary>
    public class GlobalErrorLogging
    {
        public static AppFunc Middleware(AppFunc next, ILogger log) //MidFunc signature
        {
            //This code is returning a function that takes in an environment dictionary
            //and returns a task.

            //async keyword makes it so we are returning a task without explicitly having 
            //to return one
            return async environmentDictionary =>
            {
                try
                {
                    //next is the next piece in the pipeline.  
                    //It could be the web framework or another 
                    //piece of middleware.  
                    //this just calls the next func and passes it in a the environment 
                    //dictionary and then waits for it to return a task
                    await next(environmentDictionary).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    log.Error(ex, "Unhandled exception");
                }
            };
        }
    }
}