using System;

namespace ApiGateway
{
    public class BookRequestItem
    {
        public Guid BookCatalogId { get; set; }
        public string Title { get; set; }
    }
}