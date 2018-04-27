using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BookRequest.BookRequest;
using Newtonsoft.Json;
using Platform;

namespace BookRequest.LibraryCatalog
{
    public class LibraryCatalogClient : ILibraryCatalogClient
    {
        private static readonly string LibraryCatalogBaseUrl =
            @"http://localhost:5100";

        private static readonly string GetShelfPathTemplate =
            "/books/request?bookIds=[{0}]";

        private readonly ICache _cache;
        private readonly IHttpClientFactory _httpClientFactory;

        public LibraryCatalogClient(ICache cache, IHttpClientFactory httpClientFactory)
        {
            _cache = cache;
            _httpClientFactory = httpClientFactory;
        }

        public Task<IEnumerable<BookRequestItem>> GetBookRequestItems(Guid[] bookIds)
        {
            return ResiliencePolicies.FallbackPolicy
                .WrapAsync(ResiliencePolicies.CircuitBreaker
                .WrapAsync(ResiliencePolicies.ExponentialRetryPolicy))
                .ExecuteAsync(() => GetBooksFromCatalogService(bookIds));

            //return ResiliencePolicies.CircuitBreaker.ExecuteAsync(() => GetBooksFromCatalogService(bookIds));

            //return ExponentialRetryPolicy.ExecuteAsync(() => GetBooksFromCatalogService(bookIds));
        }

        private async Task<IEnumerable<BookRequestItem>>
            GetBooksFromCatalogService(Guid[] bookIds)
        {
            //throw new Exception("Polly Test");
            var response = await
                RequestBooksFromLibraryCatalog(bookIds).ConfigureAwait(false);
            return await ConvertToBookRequestItems(response).ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> RequestBooksFromLibraryCatalog(Guid[] libraryCatalogIds)
        {
            var bookResource = string.Format(
                GetShelfPathTemplate, string.Join(",", libraryCatalogIds));

            //If there is nothing in the cache issue a new request
            if (_cache.Get(bookResource) is HttpResponseMessage response) return response;

            var httpClient = await _httpClientFactory.Create(new Uri(LibraryCatalogBaseUrl), "library_catalog_read");
            httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            response = await httpClient.GetAsync(bookResource).ConfigureAwait(false);
            AddToCache(bookResource, response);
            return response;
        }

        private void AddToCache(string resource, HttpResponseMessage response)
        {
            var cacheHeader = response
                .Headers
                .FirstOrDefault(h => h.Key == "cache-control");
            if (string.IsNullOrEmpty(cacheHeader.Key))
                return;
            var maxAge =
                CacheControlHeaderValue.Parse(cacheHeader.Value.ToString())
                    .MaxAge;
            if (maxAge.HasValue)
                _cache.Add(resource, response, maxAge.Value);
        }

        private static async Task<IEnumerable<BookRequestItem>> ConvertToBookRequestItems(
            HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            var books =
                JsonConvert.DeserializeObject<List<LibraryCatalogBook>>(await response.Content.ReadAsStringAsync()
                    .ConfigureAwait(false));
            return
                books
                    .Select(p => new BookRequestItem(0,
                        p.Id,
                        p.Title
                    ));
        }

        private class LibraryCatalogBook
        {
            public LibraryCatalogBook(Guid Id, string Title, string Summary)
            {
                this.Id = Id;
                this.Title = Title;
                this.Summary = Summary;
            }

            public Guid Id { get; }
            public string Title { get; }
            public string Summary { get; }
        }
    }
}