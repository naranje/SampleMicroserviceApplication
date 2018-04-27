using System.Collections.Generic;

namespace ApiGateway
{
    public class BookRequest
    {
        public IEnumerable<BookRequestItem> Items { get; set; }
    }
}