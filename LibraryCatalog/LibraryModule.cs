using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;
using Newtonsoft.Json;

namespace LibraryCatalog
{
    public class LibraryModule : NancyModule
    {
        public LibraryModule(ILibraryStore libraryStore) : base("/books")
        {
            Get("", _ =>
            {
                int numberOfBooks = Request.Query.numberOfBooks;

                var books = libraryStore.GetTopListOfBooks(numberOfBooks);

                return
                    Negotiate
                        .WithModel(books.Result)
                        .WithHeader("cache-control", "max-age:86400");
            });

            Get("/request", _ =>
            {
                string bookIdString = Request.Query.bookIds;
                var bookIds = ParseBookIdsFromQueryString(bookIdString);
                var books = libraryStore.GetBooksByIds(bookIds);

                return
                    Negotiate
                        .WithModel(books.Result)
                        .WithHeader("cache-control", "max-age:86400");
            });
        }
        private static IEnumerable<Guid> ParseBookIdsFromQueryString(string productIdsString)
        {
            return productIdsString.Split(',').Select(s => s.Replace("[", "").Replace("]", "")).Select(Guid.Parse);
        }
    }
}