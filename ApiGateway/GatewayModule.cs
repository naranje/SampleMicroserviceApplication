using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Nancy;
using Nancy.ModelBinding;
using Newtonsoft.Json;
using Platform;
using Serilog;

namespace ApiGateway
{
    using static Encoding;

    public class GatewayModule : NancyModule
    {
        private static Book[] _bookList;

        public GatewayModule(IHttpClientFactory clientFactory, ILogger logger)
        {
            Get("/{userid:int}", async parameters =>
            {
                var userId = (int) parameters.userid;

                var client = await clientFactory.Create(new Uri("http://localhost:5100/"), "library_catalog_read");
                client.DefaultRequestHeaders
                    .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.GetAsync("/books?numberOfBooks=10");
                var content = await response?.Content.ReadAsStringAsync();
                logger.Information(content);
                _bookList =
                    JsonConvert.DeserializeObject<List<Book>>(content).ToArray();

                client = await clientFactory.Create(new Uri("http://localhost:5200/"), "book_request_write");
                client.DefaultRequestHeaders
                    .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                response = await client.GetAsync($"/bookrequest/{userId}");
                content = await response?.Content.ReadAsStringAsync();
                logger.Information(content);
                var bookRequestProducts = GetBooksFromResponse(content);

                return View["books", new {BookList = _bookList, RequestBooks = bookRequestProducts}];
            });

            Post("/bookrequest/{userid}", async parameters =>
            {
                var bookId = this.Bind<Guid>();
                var userId = (int) parameters.userid;

                var client = await clientFactory.Create(new Uri("http://localhost:5200/"), "book_request_write");
                client.DefaultRequestHeaders
                    .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await
                    client.PostAsync(
                        $"/bookrequest/{userId}/items",
                        new StringContent(JsonConvert.SerializeObject(new[] {bookId}), UTF8, "application/json"));
                var content = await response?.Content.ReadAsStringAsync();
                logger.Information(content);
                var bookRequestProducts = GetBooksFromResponse(content);

                logger.Information("{@basket}", bookRequestProducts);
                return View["books", new {BookList = _bookList, RequestBooks = bookRequestProducts}];
            });

            Delete("/bookrequest/{userid}", async parameters =>
            {
                var bookId = this.Bind<Guid>();
                var userId = (int) parameters.userid;

                var client = await clientFactory.Create(new Uri("http://localhost:5200/"), "book_request_write");
                client.DefaultRequestHeaders
                    .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var request = new HttpRequestMessage(HttpMethod.Delete, $"/bookrequest/{userId}/items")
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new[] {bookId}), UTF8,
                        "application/json")
                };
                var response = await client.SendAsync(request);
                var content = await response?.Content.ReadAsStringAsync();
                logger.Information(content);
                var bookRequestProducts = GetBooksFromResponse(content);

                logger.Information("{@basket}", bookRequestProducts);
                return View["books", new {BookList = _bookList, RequestBooks = bookRequestProducts}];
            });
        }

        private List<Book> GetBooksFromResponse(string responseBody)
        {
            var bookRequestItems = JsonConvert.DeserializeObject<BookRequest>(responseBody)
                .Items;

            return bookRequestItems
                       ?.Select(item =>
                           new Book {Title = item.Title, Id = item.BookCatalogId})
                       ?.ToList() ?? new List<Book>();
        }
    }
}