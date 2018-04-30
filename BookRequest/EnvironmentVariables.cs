using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookRequest
{
    public static class EnvironmentVariables
    {
        public static string ConnectionString { get; } =
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=BookRequest;Integrated Security=True;Connect Timeout=600;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
    }
}
