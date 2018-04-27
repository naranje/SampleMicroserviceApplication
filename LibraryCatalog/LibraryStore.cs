using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace LibraryCatalog
{
    public class LibraryStore : ILibraryStore
    {
        //TODO: Store this in config.
        private readonly string connectionString =
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Books;Integrated Security=True;Connect Timeout=600;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public async Task<IEnumerable<LibraryCatalogBook>> GetTopListOfBooks(int numberOfProductsToGet)
        {
            string ReadItemsSql =
                $"SELECT TOP {numberOfProductsToGet} Id ,Title ,Summary ,SummaryHtml ,Authors ,Url ,SmallImageUrl ,MediumImageUrl ,LargeImageUrl ,Isbn ,Published ,Publisher ,Binding FROM dbo.Book";

            using (var conn = new SqlConnection(connectionString))
            {
                return await conn.QueryAsync<LibraryCatalogBook>(ReadItemsSql);
            }
        }

        public async Task<IEnumerable<LibraryCatalogBook>> GetBooksByIds(IEnumerable<Guid> bookIds)
        {
            string ReadItemsSql =
                $"SELECT Id ,Title ,Summary FROM dbo.Book WHERE Id in @ids";

            using (var conn = new SqlConnection(connectionString))
            {
                return await conn.QueryAsync<LibraryCatalogBook>(ReadItemsSql, new { ids = bookIds.ToArray()});
            }
        }
    }
}