using System;
using System.Collections.Generic;
using BookRequest.EventFeed;
using BookRequest.LibraryCatalog;
using Nancy;
using Nancy.ModelBinding;
using Platform;

namespace BookRequest.BookRequest
{
    public sealed class BookRequestModule : NancyModule
    {
        public BookRequestModule(
            IBookRequestStore bookRequestStore,
            ILibraryCatalogClient libraryCatalog,
            IEventStore eventStore)
            : base("/bookrequest")
        {
            Get("/{userid:int}", parameters =>
            {
                var userId = (int) parameters.userid;
                return bookRequestStore.Get(userId);
            });

            Post("/{userid:int}/items", async parameters =>
            {
                var bookIds = this.Bind<List<Guid>>();
                var userId = (int) parameters.userid;

                var bookRequest = await bookRequestStore.Get(userId).ConfigureAwait(false);
                var bookRequestItems =
                    await libraryCatalog.GetBookRequestItems(bookIds.ToArray()).ConfigureAwait(false);
                bookRequest.AddItems(bookRequestItems, eventStore);
                await bookRequestStore.Save(bookRequest);

                return bookRequest;
            });

            Delete("/{userid:int}/items", async parameters =>
            {
                var libraryCatalogIds = this.Bind<Guid[]>();
                var userId = (int) parameters.userid;

                var bookRequest = await bookRequestStore.Get(userId).ConfigureAwait(false);
                bookRequest.RemoveItems(libraryCatalogIds, eventStore);
                await bookRequestStore.Save(bookRequest);

                return bookRequest;
            });
        }
    }
}
