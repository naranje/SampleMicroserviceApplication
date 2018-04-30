using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookRequest.BookRequest
{
    public static class Repository
    {
        public const string ReadCartSql =
            @"select Id, UserId  from BookRequest
where UserId=@UserId";

        public const string ReadItemsSql =
            @"select BookRequestItems.BookRequestId, BookRequestItems.BookCatalogId, BookRequestItems.Title from BookRequest, BookRequestItems
where BookRequestItems.BookRequestId = BookRequest.ID
and BookRequest.UserId=@UserId";

        public const string DeleteAllForBookRequestSql =
            @"delete item from BookRequestItems item
inner join BookRequest cart on item.BookRequestId = cart.ID
and cart.UserId=@UserId";

        public const string AddAllForBookRequestSql =
            @"insert into BookRequestItems 
(BookRequestId, BookCatalogId, Title)
values 
(@BookRequestId, @BookCatalogId, @Title)";
    }
}
