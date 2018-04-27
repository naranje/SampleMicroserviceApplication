using System.Security.Claims;
using LibOwin;
using Nancy;
using Nancy.Owin;
using Nancy.TinyIoc;

namespace Platform
{
    public static class MicroservicePlatform
  {
    private static string _tokenUrl;
    private static string _clientName;
    private static string _clientSecret;

    public static void Configure(string tokenUrl, string clientName, string clientSecret)
    {
      _tokenUrl = tokenUrl;
      _clientName = clientName;
      _clientSecret = clientSecret;
    }

    public static TinyIoCContainer UseHttpClientFactory(this TinyIoCContainer self, NancyContext context)
    {
      //Reads the correlation token from the OWIN environment  
      var correlationToken =
        context.GetOwinEnvironment()?["correlationToken"] as string;
      object key = null;
        //Reads the end user from the OWIN environment
      context.GetOwinEnvironment()?.TryGetValue(OwinConstants.RequestUser, out key);
      var principal = key as ClaimsPrincipal;
        //Gets the end user's identity token from the user object
      var idToken = principal?.FindFirst("id_token");
        //Registers the HttpClientFactory as a per-request dependency in Nancy's container
      self.Register<IHttpClientFactory>(new HttpClientFactory(_tokenUrl, _clientName, _clientSecret, correlationToken ?? "", idToken?.Value ?? ""));
      return self;
    }
  }
}