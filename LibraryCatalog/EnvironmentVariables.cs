namespace LibraryCatalog
{
    public static class EnvironmentVariables
    {
        public static string ConnectionString { get; } = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Books;Integrated Security=True;Connect Timeout=600;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
    }
}
