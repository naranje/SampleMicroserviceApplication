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
        public async Task<IEnumerable<LibraryCatalogBook>> GetTopListOfBooks(int numberOfProductsToGet)
        {
            string readItemsSql =
                $"SELECT TOP {numberOfProductsToGet} Id ,Title ,Summary ,SummaryHtml ,Authors ,Url ,SmallImageUrl ,MediumImageUrl ,LargeImageUrl ,Isbn ,Published ,Publisher ,Binding FROM dbo.Book";

            using (var conn = new SqlConnection(EnvironmentVariables.ConnectionString))
            {
                return await conn.QueryAsync<LibraryCatalogBook>(readItemsSql);
            }
        }

        public async Task<IEnumerable<LibraryCatalogBook>> GetBooksByIds(IEnumerable<Guid> bookIds)
        {
            string readItemsSql =
                $"SELECT Id ,Title ,Summary FROM dbo.Book WHERE Id in @ids";

            using (var conn = new SqlConnection(EnvironmentVariables.ConnectionString))
            {
                return await conn.QueryAsync<LibraryCatalogBook>(readItemsSql, new { ids = bookIds.ToArray()});
            }
        }
    }
}