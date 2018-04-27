using System;
using System.Collections.Generic;
using System.Linq;
using BookRequest.EventFeed;

namespace BookRequest.BookRequest
{
    public class BookRequest
    {
        private readonly HashSet<BookRequestItem> _items = new HashSet<BookRequestItem>();

        public long UserId { get; }
        public int BookRequestId { get; }
        public IEnumerable<BookRequestItem> Items => _items;

        public BookRequest(Int32 Id, Int64 UserId)
        {
            this.UserId = UserId;
            BookRequestId = Id;
        }

        public BookRequest(int userId, int bookRequestId, IEnumerable<BookRequestItem> items)
        {
            UserId = userId;
            BookRequestId = bookRequestId;
            foreach (var item in items) _items.Add(item);
        }

        public void AddItems(
            IEnumerable<BookRequestItem> shoppingCartItems,
            IEventStore eventStore)
        {
            foreach (var item in shoppingCartItems)
            {
                item.BookRequestId = BookRequestId;
                if (_items.Add(item))
                    eventStore.Raise(
                        "BookRequestItemAdded",
                        new {UserId, item});
            }
        }

        public void RemoveItems(
            Guid[] libraryCatalogIds,
            IEventStore eventStore)
        {
            _items.RemoveWhere(i => libraryCatalogIds.Contains(i.BookCatalogId));
        }
    }
}
