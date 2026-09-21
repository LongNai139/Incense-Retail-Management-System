using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SV22T1080045.Shop.DataLayers
{
    /// <summary>
    /// CHỈ dùng cho lệnh "Add-Migration" và "Update-Database" trong Package Manager Console.
    /// KHÔNG dùng để inject vào service — dùng ShopDbContext trực tiếp qua DI.
    /// </summary>
    public class ShopDbContextFactory : IDesignTimeDbContextFactory<ShopDbContext>
    {
        public ShopDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ShopDbContext>();

            // ⚠️ Chỉ dùng khi chạy migration — chuỗi kết nối hardcode ở đây là bình thường
            var connectionString =
                "Server=.;Database=ShopIncenseDB;Trusted_Connection=True;" +
                "TrustServerCertificate=True;MultipleActiveResultSets=true";

            optionsBuilder.UseSqlServer(connectionString);
            return new ShopDbContext(optionsBuilder.Options);
        }
    }
}