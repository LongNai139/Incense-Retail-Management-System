using Dapper;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.DataLayers
{
    public interface IPhoneOtpDAL
    {
        string Generate(string phone, string purpose);
        bool Verify(string phone, string purpose, string code);
        void CleanupExpired();
    }

    public class PhoneOtpDAL : _BaseDAL, IPhoneOtpDAL
    {
        public PhoneOtpDAL(string connectionString) : base(connectionString) { }

        // ── TẠO OTP MỚI ──────────────────────────────────────────────────────
        public string Generate(string phone, string purpose)
        {
            using var conn = OpenConnection();

            // Xóa OTP cũ cùng phone + purpose
            conn.Execute(
                "DELETE FROM PhoneOtps WHERE Phone = @phone AND Purpose = @purpose",
                new { phone, purpose });

            var code = Random.Shared.Next(100000, 999999).ToString();

            conn.Execute(@"
                INSERT INTO PhoneOtps (Phone, OtpCode, Purpose, ExpiresAt, IsUsed, FailCount, CreatedAt)
                VALUES (@phone, @code, @purpose, DATEADD(MINUTE,5,GETDATE()), 0, 0, GETDATE())",
                new { phone, code, purpose });

            return code;
        }

        // ── XÁC THỰC OTP ─────────────────────────────────────────────────────
        public bool Verify(string phone, string purpose, string code)
        {
            using var conn = OpenConnection();

            var otp = conn.QueryFirstOrDefault<PhoneOtp>(@"
                SELECT TOP 1 *
                FROM   PhoneOtps
                WHERE  Phone = @phone AND Purpose = @purpose AND IsUsed = 0
                ORDER BY CreatedAt DESC",
                new { phone, purpose });

            if (otp == null) return false;
            if (!otp.IsValid()) return false;

            if (otp.OtpCode != code)
            {
                // Tăng FailCount
                conn.Execute(
                    "UPDATE PhoneOtps SET FailCount = FailCount + 1 WHERE Id = @id",
                    new { id = otp.Id });
                return false;
            }

            // Đúng → đánh dấu đã dùng
            conn.Execute(
                "UPDATE PhoneOtps SET IsUsed = 1 WHERE Id = @id",
                new { id = otp.Id });
            return true;
        }

        // ── DỌN DẸP OTP HẾT HẠN ─────────────────────────────────────────────
        public void CleanupExpired()
        {
            using var conn = OpenConnection();
            conn.Execute(
                "DELETE FROM PhoneOtps WHERE ExpiresAt < GETDATE() OR IsUsed = 1");
        }
    }
}