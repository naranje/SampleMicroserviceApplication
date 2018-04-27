using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;

namespace BookRequest.BookRequest
{
    public class BookRequestStore : IBookRequestStore
    {
        //TODO: Store this in config.
        private readonly string connectionString =
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=BookRequest;Integrated Security=True;Connect Timeout=600;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        private const string ReadCartSql =
            @"select Id, UserId  from BookRequest
where UserId=@UserId";

        private const string ReadItemsSql =
            @"select BookRequestItems.BookRequestId, BookRequestItems.BookCatalogId, BookRequestItems.Title from BookRequest, BookRequestItems
where BookRequestItems.BookRequestId = BookRequest.ID
and BookRequest.UserId=@UserId";

        private const string DeleteAllForBookRequestSql =
            @"delete item from BookRequestItems item
inner join BookRequest cart on item.BookRequestId = cart.ID
and cart.UserId=@UserId";

        private const string AddAllForBookRequestSql =
            @"insert into BookRequestItems 
(BookRequestId, BookCatalogId, Title)
values 
(@BookRequestId, @BookCatalogId, @Title)";


        public async Task<BookRequest> Get(int userId)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                var bookRequest = await conn.QueryFirstAsync<BookRequest>(ReadCartSql,
                    new {UserId = userId});

                var currentBookRequestItems = await
                    conn.QueryAsync<BookRequestItem>(
                        ReadItemsSql,
                        new {UserId = userId});

                return new BookRequest(userId, bookRequest.BookRequestId, currentBookRequestItems);
            }
        }

        public async Task Save(BookRequest bookRequest)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        await conn.ExecuteAsync(
                            DeleteAllForBookRequestSql,
                            new {bookRequest.UserId},
                            tx).ConfigureAwait(false);

                        await conn.ExecuteAsync(
                            AddAllForBookRequestSql,
                            bookRequest.Items,
                            tx).ConfigureAwait(false);

                        tx.Commit();
                    }
                    catch (Exception)
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}