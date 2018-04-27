using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace Platform
{
    public class HttpClientFactory : IHttpClientFactory
    {
        private readonly TokenClient _tokenClient;
        private readonly string _correlationToken;
        private readonly string _idToken;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokenUrl">Url of the token endpoint in the Login microservice</param>
        /// <param name="clientName">Client name used to obtain an access token from the token endpoint</param>
        /// <param name="clientSecret">Client secret used to obtain an access token from the token endpoint</param>
        /// <param name="correlationToken">Per-request correlation token coming from a piece of middleware</param>
        /// <param name="idToken">Token with the end-user's identity</param>
        public HttpClientFactory(string tokenUrl, string clientName, string clientSecret, string correlationToken,
            string idToken)
        {
            _tokenClient = new TokenClient(tokenUrl, clientName, clientSecret);
            _correlationToken = correlationToken;
            _idToken = idToken;
        }

        public async Task<HttpClient> Create(Uri uri, string requestScope)
        {
            //Requests an authorization token from the Login microservice allowing calls that require the scope
            //in requestScope
            var response = await _tokenClient.RequestClientCredentialsAsync(requestScope).ConfigureAwait(false);
            //Prepares the client to make requests to uri
            var client = new HttpClient() {BaseAddress = uri};
            client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //Adds the authorization token to a request header
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", response.AccessToken);
            
            //Adds the correlation token to the request header
            client.DefaultRequestHeaders.Add("Correlation-Token", _correlationToken);
            if (!string.IsNullOrEmpty(_idToken))
                //Adds the end user's identity to a request header
                client.DefaultRequestHeaders.Add("library-member", _idToken);
            return client;
        }
    }
}