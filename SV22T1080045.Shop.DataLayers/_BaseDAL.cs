using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace SV22T1080045.Shop.DataLayers
{
    public abstract class BaseDAL
    {
        protected string connectionString;
        /// <param name="connectionString">Chuỗi tham số kết nối đến CSDL</param>
        public BaseDAL(string connectionString) 
        {
            this.connectionString = connectionString;
        }
        /// <summary>
        /// Mở kết nối đến CSDL
        /// </summary>
        /// <returns></returns>
        protected SqlConnection OpenConnection()
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = connectionString;
            connection.Open();
            return connection;
        }
        /// <summary>
        /// Mở kết nối tới CDSL bất đồng bộ
        /// </summary>
        /// <returns></returns>
        protected async Task<SqlConnection> OpenConnectionAsync()
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = connectionString;
            await connection.OpenAsync();
            return connection;
        }
    }
}
