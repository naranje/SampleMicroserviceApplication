using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookRequest.BookRequest;

namespace BookRequest.LibraryCatalog
{
    public interface ILibraryCatalogClient
    {
        Task<IEnumerable<BookRequestItem>> GetBookRequestItems(Guid[] bookIds);
    }
}