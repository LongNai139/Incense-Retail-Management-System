using Dapper;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.DataLayers
{
    public class AccountDAL : BaseDAL
    {
        public AccountDAL(string connectionString) : base(connectionString) { }

        // Kiểm tra đăng nhập
        public Customer? Login(string email, string password)
        {
            using var conn = OpenConnection();
            var sql = "SELECT * FROM Customers WHERE Email = @Email AND Password = @Password";
            return conn.QueryFirstOrDefault<Customer>(sql, new { Email = email, Password = password });
        }

        // Kiểm tra email đã tồn tại chưa (Dùng khi đăng ký)
        public bool EmailExists(string email)
        {
            using var conn = OpenConnection();
            var count = conn.ExecuteScalar<int>("SELECT COUNT(*) FROM Customers WHERE Email = @Email", new { Email = email });
            return count > 0;
        }

        // Đăng ký tài khoản mới
        public bool Register(Customer data)
        {
            using var conn = OpenConnection();
            var sql = @"INSERT INTO Customers(CustomerName, Email, Password, Phone, Address)
                        VALUES(@CustomerName, @Email, @Password, @Phone, @Address)";
            return conn.Execute(sql, data) > 0;
        }

        // Lấy thông tin khách hàng theo ID
        public Customer? GetCustomerById(int id)
        {
            using var conn = OpenConnection();
            return conn.QueryFirstOrDefault<Customer>("SELECT * FROM Customers WHERE CustomerID = @Id", new { Id = id });
        }

        // Cập nhật thông tin cá nhân
        public bool UpdateProfile(Customer data)
        {
            using var conn = OpenConnection();
            var sql = @"UPDATE Customers 
                        SET CustomerName = @CustomerName, Phone = @Phone, Address = @Address
                        WHERE CustomerID = @CustomerID";
            return conn.Execute(sql, data) > 0;
        }

        // Đổi mật khẩu
        public bool ChangePassword(int id, string newPassword)
        {
            using var conn = OpenConnection();
            var sql = "UPDATE Customers SET Password = @Password WHERE CustomerID = @Id";
            return conn.Execute(sql, new { Password = newPassword, Id = id }) > 0;
        }
    }
}