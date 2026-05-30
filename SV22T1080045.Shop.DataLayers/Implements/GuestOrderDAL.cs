using Dapper;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.DomainModels;
using System.Security.Cryptography;
using System.Text;

namespace SV22T1080045.Shop.DataLayers.Implements
{
    public class GuestOrderDAL : _BaseDAL, IGuestOrderDAL
    {
        public GuestOrderDAL(string connectionString) : base(connectionString) { }

        // ── HASH SĐT (SHA-256, không lưu SĐT gốc) ────────────────────────────
        public static string HashPhone(string phone)
        {
            var normalized = phone.Trim().Replace(" ", "")
                                  .Replace("+84", "0");
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(normalized));
            return Convert.ToHexString(bytes).ToLower();
        }

        private static string LastFour(string phone)
        {
            var digits = phone.Replace(" ", "").Replace("+84", "0");
            return digits.Length >= 4 ? digits[^4..] : digits;
        }

        public void Save(int orderId, string phone)
        {
            using var conn = OpenConnection();
            var exists = conn.ExecuteScalar<int>(
                "SELECT COUNT(1) FROM GuestOrders WHERE OrderId = @orderId",
                new { orderId });
            if (exists > 0) return;

            conn.Execute(@"
                INSERT INTO GuestOrders (OrderId, PhoneHash, PhoneLastFour, CreatedAt)
                VALUES (@orderId, @phoneHash, @phoneLastFour, GETDATE())",
                new
                {
                    orderId,
                    phoneHash = HashPhone(phone),
                    phoneLastFour = LastFour(phone)
                });
        }

        public List<Order> GetOrdersByPhone(string phone)
        {
            using var conn = OpenConnection();
            var hash = HashPhone(phone);

            return conn.Query<Order>(@"
                SELECT o.*
                FROM   Orders o
                JOIN   GuestOrders g ON g.OrderId = o.Id
                WHERE  g.PhoneHash = @hash
                ORDER BY o.OrderDate DESC",
                new { hash }).ToList();
        }

        public bool MergeToCustomer(string phone, int customerId)
        {
            using var conn = OpenConnection();
            var hash = HashPhone(phone);

            int rows = conn.Execute(@"
                UPDATE o SET o.CustomerId = @customerId
                FROM   Orders o
                JOIN   GuestOrders g ON g.OrderId = o.Id
                WHERE  g.PhoneHash = @hash
                  AND  (o.CustomerId = 0 OR o.CustomerId IS NULL)",
                new { hash, customerId });

            conn.Execute(@"
                UPDATE GuestOrders
                SET    ConvertedCustomerId = @customerId
                WHERE  PhoneHash = @hash AND ConvertedCustomerId IS NULL",
                new { hash, customerId });

            return rows > 0;
        }
    }
}
