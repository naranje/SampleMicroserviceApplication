using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;

namespace BookRequest.BookRequest
{
    public class BookRequestStore : IBookRequestStore
    {
        public async Task<BookRequest> Get(int userId)
        {
            using (var conn = new SqlConnection(EnvironmentVariables.ConnectionString))
            {
                var bookRequest = await conn.QueryFirstAsync<BookRequest>(Repository.ReadCartSql,
                    new {UserId = userId});

                var currentBookRequestItems = await
                    conn.QueryAsync<BookRequestItem>(Repository.ReadItemsSql,
                        new {UserId = userId});

                return new BookRequest(userId, bookRequest.BookRequestId, currentBookRequestItems);
            }
        }

        public async Task Save(BookRequest bookRequest)
        {
            using (var conn = new SqlConnection(EnvironmentVariables.ConnectionString))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        await conn.ExecuteAsync(Repository.DeleteAllForBookRequestSql,
                            new {bookRequest.UserId},
                            tx).ConfigureAwait(false);

                        await conn.ExecuteAsync(Repository.AddAllForBookRequestSql,
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