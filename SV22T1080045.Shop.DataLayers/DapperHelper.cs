using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace SV22T1080045.Shop.DataLayers
{
    public static class DapperHelper
    {
        public static System.Data.IDbConnection GetConnection(this DbContext context)
        {
            var connection = new SqlConnection(context.Database.GetConnectionString());
            connection.Open();
            return connection;
        }
    }
}