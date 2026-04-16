using Dapper;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.DataLayers
{
    /// <summary>
    /// VoucherDAL dùng Dapper — nhất quán với OrderDAL, GuestOrderDAL.
    /// KHÔNG dùng EF Core hay ShopDbContextFactory ở đây.
    /// </summary>
    public class VoucherDAL : _BaseDAL
    {
        public VoucherDAL(string connectionString) : base(connectionString) { }

        public Voucher? GetByCode(string code)
        {
            using var conn = OpenConnection();
            return conn.QueryFirstOrDefault<Voucher>(
                "SELECT * FROM Vouchers WHERE Code = @code AND IsActive = 1",
                new { code = code.Trim().ToUpper() });
        }

        public bool Use(string code)
        {
            using var conn = OpenConnection();
            return conn.Execute(
                "UPDATE Vouchers SET UsedCount = UsedCount + 1 WHERE Code = @code",
                new { code = code.Trim().ToUpper() }) > 0;
        }

        public int Add(Voucher v)
        {
            using var conn = OpenConnection();
            v.Code = v.Code.Trim().ToUpper();
            return conn.ExecuteScalar<int>(@"
                INSERT INTO Vouchers
                    (Code, Description, DiscountType, DiscountValue, MaxDiscount,
                     MinOrderAmount, ExpiresAt, MaxUsage, UsedCount, IsActive)
                VALUES
                    (@Code, @Description, @DiscountType, @DiscountValue, @MaxDiscount,
                     @MinOrderAmount, @ExpiresAt, @MaxUsage, 0, 1);
                SELECT SCOPE_IDENTITY();", v);
        }

        public bool Update(Voucher v)
        {
            using var conn = OpenConnection();
            v.Code = v.Code.Trim().ToUpper();
            return conn.Execute(@"
                UPDATE Vouchers SET
                    Code=@Code, Description=@Description,
                    DiscountType=@DiscountType, DiscountValue=@DiscountValue,
                    MaxDiscount=@MaxDiscount, MinOrderAmount=@MinOrderAmount,
                    ExpiresAt=@ExpiresAt, MaxUsage=@MaxUsage, IsActive=@IsActive
                WHERE Id=@Id", v) > 0;
        }

        public bool Delete(int id)
        {
            using var conn = OpenConnection();
            return conn.Execute(
                "UPDATE Vouchers SET IsActive = 0 WHERE Id = @id",
                new { id }) > 0;
        }
    }
}