using Dapper;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.DataLayers
{
    public class AccountDAL : _BaseDAL
    {
        public AccountDAL(string connectionString) : base(connectionString) { }

        // Kiểm tra đăng nhập
        public Customer? Login(string email, string password)
        {
            using var conn = OpenConnection();
            var sql = "SELECT * FROM Customers WHERE Email = @Email AND PasswordHash = @PasswordHash";
            return conn.QueryFirstOrDefault<Customer>(sql, new { Email = email, PasswordHash = password });
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
            var sql = @"INSERT INTO Customers(FullName, Email, PasswordHash, PhoneNumber, Address, Role, CreatedTime, IsDeleted)
                        VALUES(@FullName, @Email, @PasswordHash, @PhoneNumber, @Address, @Role, GETDATE(), 0)";
            return conn.Execute(sql, data) > 0;
        }

        // Lấy thông tin khách hàng theo ID
        public Customer? GetCustomerById(int id)
        {
            using var conn = OpenConnection();
            return conn.QueryFirstOrDefault<Customer>("SELECT * FROM Customers WHERE Id = @Id", new { Id = id });
        }

        // Cập nhật thông tin cá nhân
        public bool UpdateProfile(Customer data)
        {
            using var conn = OpenConnection();
            var sql = @"UPDATE Customers
                        SET FullName = @FullName, PhoneNumber = @PhoneNumber, Address = @Address
                        WHERE Id = @Id";
            return conn.Execute(sql, data) > 0;
        }

        // Đổi mật khẩu
        public bool ChangePassword(int id, string newPassword)
        {
            using var conn = OpenConnection();
            var sql = "UPDATE Customers SET PasswordHash = @PasswordHash WHERE Id = @Id";
            return conn.Execute(sql, new { PasswordHash = newPassword, Id = id }) > 0;
        }
    }
}
