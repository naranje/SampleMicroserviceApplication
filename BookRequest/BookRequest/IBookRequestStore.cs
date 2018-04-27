using System.Collections.Generic;

namespace BookRequest.BookRequest
{
  using System.Threading.Tasks;
  
  public interface IBookRequestStore
  {
    Task<BookRequest> Get(int userId);
    Task Save(BookRequest bookRequest);
  }
}