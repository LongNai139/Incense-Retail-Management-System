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
        public DbSet<GuestOrder> GuestOrders { get; set; }
        public DbSet<PhoneOtp> PhoneOtps { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<ProductInventory> ProductInventories { get; set; }

        // 2. Cấu hình thêm (Ví dụ: Tự tạo tài khoản Admin mặc định)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 👇 1. Cấu hình kiểu dữ liệu cho TIỀN TỆ (Sửa lỗi warning màu vàng)
            modelBuilder.Entity<Order>().Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Order>().Property(o => o.Note).HasMaxLength(500);
            modelBuilder.Entity<Order>().Property(o => o.PaymentTransactionNo).HasMaxLength(100);
            modelBuilder.Entity<Order>().Property(o => o.PaymentBankCode).HasMaxLength(50);
            modelBuilder.Entity<Order>().Property(o => o.PaymentResponseCode).HasMaxLength(20);
            modelBuilder.Entity<OrderDetail>().Property(od => od.UnitPrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Product>().Property(p => p.ImportPrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Product>().Property(p => p.SalePrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Product>().Property(p => p.DiscountPercent).HasColumnType("decimal(5,2)");
            modelBuilder.Entity<Product>().Property(p => p.OriginalPrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Product>().Property(p => p.PriceAfterDiscount).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ProductInventory>(e =>
            {
                e.ToTable("ProductInventories");
                e.HasKey(x => x.ProductId);
                e.Property(x => x.Quantity).HasDefaultValue(0);
                e.Property(x => x.LowStockThreshold).HasDefaultValue(5);
                e.Property(x => x.UpdatedTime).HasDefaultValueSql("GETDATE()");

                e.HasOne(x => x.Product)
                 .WithOne(x => x.Inventory)
                 .HasForeignKey<ProductInventory>(x => x.ProductId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // 👇 2. Seed Data (Tạo Admin mẫu - Giữ nguyên cái cũ của bạn)
            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    Id = 1,
                    CustomerName = "Quản trị viên",
                    Phone = "0909123456",
                    Password = "123",
                    Role = "Admin",
                    Address = "Cửa hàng Hương Trầm",
                    IsDeleted = false,
                    CreatedTime = System.DateTime.Now
                }
            );

            // GuestOrders
            modelBuilder.Entity<GuestOrder>(e =>
            {
                e.ToTable("GuestOrders");
                e.HasKey(x => x.OrderId); // 1 đơn → 1 bản ghi guest

                e.Property(x => x.PhoneHash).HasMaxLength(64).IsRequired();
                e.Property(x => x.PhoneLastFour).HasMaxLength(4).IsRequired();

                e.HasIndex(x => x.PhoneHash); // query nhanh theo SĐT hash

                e.HasOne(x => x.Order)
                 .WithOne()
                 .HasForeignKey<GuestOrder>(x => x.OrderId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // PhoneOtps
            modelBuilder.Entity<PhoneOtp>(e =>
            {
                e.ToTable("PhoneOtps");
                e.Property(x => x.Phone).HasMaxLength(15).IsRequired();
                e.Property(x => x.OtpCode).HasMaxLength(6).IsRequired();
                e.Property(x => x.Purpose).HasMaxLength(20).IsRequired();

                e.HasIndex(x => new { x.Phone, x.Purpose });
            });

            // Vouchers
            modelBuilder.Entity<Voucher>(e =>
            {
                e.ToTable("Vouchers");
                e.Property(x => x.Code).HasMaxLength(50).IsRequired();
                e.Property(x => x.DiscountValue).HasPrecision(18, 2);
                e.Property(x => x.MaxDiscount).HasPrecision(18, 2);
                e.Property(x => x.MinOrderAmount).HasPrecision(18, 2);

                e.HasIndex(x => x.Code).IsUnique();
            });
        }
    }
}
