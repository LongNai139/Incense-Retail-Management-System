using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SV22T1080045.Shop.DataLayers
{
    // Class này chỉ dùng để chạy lệnh Migration (EF Core sẽ tự tìm thấy nó)
    public class ShopDbContextFactory : IDesignTimeDbContextFactory<ShopDbContext>
    {
        public ShopDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ShopDbContext>();

            // Thay đổi chuỗi kết nối ở đây cho khớp với máy bạn
            var connectionString = "Server=.;Database=ShopIncenseDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";

            optionsBuilder.UseSqlServer(connectionString);

            return new ShopDbContext(optionsBuilder.Options);
        }
    }
}