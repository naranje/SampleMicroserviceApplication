using System.Threading.Tasks;
using LibOwin;

namespace Auth
{
    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, Task>;

    /// <summary>
    /// Authorization middleware
    /// </summary>
  public class Authorization
  {
    public static AppFunc Middleware(AppFunc next, string requiredScope)
    {
      return env =>
      {
        var ctx = new OwinContext(env);
        var principal = ctx.Request.User;
          //Calls next in the pipeline only if the request has the required scope
          //The Login service provides scopes for microservices, allowing them to collaborate
          //The scope is the proof that the request is allowed by Login, which is why microservices
          //should check incoming requests for all required scopes 
        if (principal.HasClaim("scope", requiredScope))
          return next(env);
          //If the request doesn't have the required scope, return a 403 Forbidden
        ctx.Response.StatusCode = 403;
        return Task.FromResult(0);
      };
    }
  }
}
