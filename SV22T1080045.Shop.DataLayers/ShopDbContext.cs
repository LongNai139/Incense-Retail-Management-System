using Microsoft.EntityFrameworkCore;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.DataLayers
{
    public class ShopDbContext : DbContext
    {
        public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options)
        {
        }

        // 1. Khai báo đầy đủ các bảng sẽ tạo trong SQL Server
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Customer> Customers { get; set; } // Gộp cả Admin và Khách
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        // 2. Cấu hình thêm (Ví dụ: Tự tạo tài khoản Admin mặc định)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 👇 1. Cấu hình kiểu dữ liệu cho TIỀN TỆ (Sửa lỗi warning màu vàng)
            modelBuilder.Entity<Order>().Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<OrderDetail>().Property(od => od.UnitPrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Product>().Property(p => p.OriginalPrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Product>().Property(p => p.PriceAfterDiscount).HasColumnType("decimal(18,2)");

            // 👇 2. Seed Data (Tạo Admin mẫu - Giữ nguyên cái cũ của bạn)
            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    Id = 1,
                    FullName = "Quản trị viên",
                    PhoneNumber = "0909123456",
                    PasswordHash = "123",
                    Role = "Admin",
                    Address = "Cửa hàng Hương Trầm",
                    IsDeleted = false,
                    CreatedTime = System.DateTime.Now
                }
            );
        }
    }
}