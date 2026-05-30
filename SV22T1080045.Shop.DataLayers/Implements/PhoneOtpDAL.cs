using Dapper;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.DataLayers.Implements
{
    public class PhoneOtpDAL : _BaseDAL, IPhoneOtpDAL
    {
        public PhoneOtpDAL(string connectionString) : base(connectionString) { }

        public string Generate(string phone, string purpose)
        {
            using var conn = OpenConnection();

            conn.Execute(
                "DELETE FROM PhoneOtps WHERE Phone = @phone AND Purpose = @purpose",
                new { phone, purpose });

            var code = Random.Shared.Next(100000, 999999).ToString();

            conn.Execute(@"
                INSERT INTO PhoneOtps (Phone, OtpCode, Purpose, ExpiresAt, IsUsed, FailCount, CreatedAt, CreatedTime, IsDeleted)
                VALUES (@phone, @code, @purpose, DATEADD(MINUTE,5,GETDATE()), 0, 0, GETDATE(), GETDATE(), 0)",
                new { phone, code, purpose });

            return code;
        }

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
                conn.Execute(
                    "UPDATE PhoneOtps SET FailCount = FailCount + 1 WHERE Id = @id",
                    new { id = otp.Id });
                return false;
            }

            conn.Execute(
                "UPDATE PhoneOtps SET IsUsed = 1 WHERE Id = @id",
                new { id = otp.Id });
            return true;
        }

        public void CleanupExpired()
        {
            using var conn = OpenConnection();
            conn.Execute("DELETE FROM PhoneOtps WHERE ExpiresAt < GETDATE() OR IsUsed = 1");
        }
    }
}
