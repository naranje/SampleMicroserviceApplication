using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Platform
{
    public interface IHttpClientFactory
    {
        Task<HttpClient> Create(Uri uri, string requestScope);
    }
}