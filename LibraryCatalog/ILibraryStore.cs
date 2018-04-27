using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryCatalog
{
    public interface ILibraryStore
    {
        Task<IEnumerable<LibraryCatalogBook>> GetTopListOfBooks(int numberOfProductsToGet);
        Task<IEnumerable<LibraryCatalogBook>> GetBooksByIds(IEnumerable<Guid> bookIds);
    }
}