using Microsoft.EntityFrameworkCore;
using SV22T1080045.Shop.DomainModels;
using SV22T1080045.Shop.DomainModels.Warehouse;

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
        
        // Warehouse management tables
        public DbSet<StockTransaction> StockTransactions { get; set; }
        public DbSet<CustomerDebt> CustomerDebts { get; set; }
        public DbSet<CustomerDebtTransaction> CustomerDebtTransactions { get; set; }


        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<RoleGroup> RoleGroups { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }

        // 2. Cấu hình thêm (Ví dụ: Tự tạo tài khoản Admin mặc định)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 👇 1. Cấu hình kiểu dữ liệu cho TIỀN TỆ (Sửa lỗi warning màu vàng)
            modelBuilder.Entity<Order>().Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Order>().Property(o => o.DiscountAmount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Order>().Property(o => o.FinalAmount).HasColumnType("decimal(18,2)");
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

            // Customer index optimization
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Phone)
                .HasDatabaseName("IX_Customers_Phone");

            modelBuilder.Entity<Customer>()
                .HasIndex(c => new { c.Phone, c.IsDeleted })
                .HasDatabaseName("IX_Customers_Phone_IsDeleted");

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

            // StockTransactions
            modelBuilder.Entity<StockTransaction>(e =>
            {
                e.ToTable("StockTransactions");
                e.Property(x => x.TransactionType).HasMaxLength(50).IsRequired();
                e.Property(x => x.UnitPrice).HasPrecision(18, 2);
                e.Property(x => x.ReferenceNumber).HasMaxLength(100);
                e.Property(x => x.Reason).HasMaxLength(500);

                e.HasIndex(x => x.ProductId);
                e.HasIndex(x => x.RelatedOrderDetailId);
                e.HasIndex(x => x.TransactionDate);
                e.HasIndex(x => x.TransactionType);
            });



            // CustomerDebts
            modelBuilder.Entity<CustomerDebt>(e =>
            {
                e.ToTable("CustomerDebts");
                e.Property(x => x.OrderCode).HasMaxLength(50);
                e.Property(x => x.OrderAmount).HasPrecision(18, 2);
                e.Property(x => x.DebtAmount).HasPrecision(18, 2);
                e.Property(x => x.PaidAmount).HasPrecision(18, 2);
                e.Property(x => x.PaymentReference).HasMaxLength(100);
                e.Property(x => x.Status).HasMaxLength(50).IsRequired();
                e.Property(x => x.Notes).HasMaxLength(500);

                e.HasIndex(x => x.CustomerId);
                e.HasIndex(x => x.DueDate);
                e.HasIndex(x => x.Status);
                e.HasIndex(x => new { x.CustomerId, x.OrderId })
                    .IsUnique()
                    .HasFilter("[OrderId] IS NOT NULL");
            });

            // CustomerDebtTransactions
            modelBuilder.Entity<CustomerDebtTransaction>(e =>
            {
                e.ToTable("CustomerDebtTransactions");
                e.Property(x => x.TransactionType).HasMaxLength(20).IsRequired();
                e.Property(x => x.Amount).HasPrecision(18, 2);
                e.Property(x => x.BalanceAfter).HasPrecision(18, 2);
                e.Property(x => x.Reference).HasMaxLength(100);
                e.Property(x => x.Description).HasMaxLength(500);

                e.HasIndex(x => x.CustomerDebtId);
                e.HasIndex(x => x.OrderId);
                e.HasIndex(x => x.TransactionDate);
                e.HasIndex(x => x.CreatedTime);
            });

            // ============================================================================
            // Permission System Configuration
            // ============================================================================

            // Roles
            modelBuilder.Entity<Role>(e =>
            {
                e.ToTable("Roles");
                e.Property(x => x.Name).HasMaxLength(50).IsRequired();
                e.Property(x => x.DisplayName).HasMaxLength(100).IsRequired();
                e.Property(x => x.Description).HasMaxLength(500);

                e.HasIndex(x => x.Name).IsUnique();
            });

            // Permissions
            modelBuilder.Entity<Permission>(e =>
            {
                e.ToTable("Permissions");
                e.Property(x => x.Code).HasMaxLength(100).IsRequired();
                e.Property(x => x.DisplayName).HasMaxLength(100).IsRequired();
                e.Property(x => x.Description).HasMaxLength(500);
                e.Property(x => x.Module).HasMaxLength(50);

                e.HasIndex(x => x.Code).IsUnique();
                e.HasIndex(x => x.Module);
            });

            // RolePermissions (Many-to-Many)
            modelBuilder.Entity<RolePermission>(e =>
            {
                e.ToTable("RolePermissions");
                e.HasKey(x => new { x.RoleId, x.PermissionId });

                e.HasOne(x => x.Role)
                 .WithMany(x => x.RolePermissions)
                 .HasForeignKey(x => x.RoleId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Permission)
                 .WithMany(x => x.RolePermissions)
                 .HasForeignKey(x => x.PermissionId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasIndex(x => x.PermissionId);
            });

            // Groups
            modelBuilder.Entity<Group>(e =>
            {
                e.ToTable("Groups");
                e.Property(x => x.Name).HasMaxLength(100).IsRequired();
                e.Property(x => x.Description).HasMaxLength(500);

                e.HasIndex(x => x.Name).IsUnique();
            });

            // RoleGroups (Many-to-Many)
            modelBuilder.Entity<RoleGroup>(e =>
            {
                e.ToTable("RoleGroups");
                e.HasKey(x => new { x.GroupId, x.RoleId });

                e.HasOne(x => x.Group)
                 .WithMany(x => x.RoleGroups)
                 .HasForeignKey(x => x.GroupId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Role)
                 .WithMany(x => x.RoleGroups)
                 .HasForeignKey(x => x.RoleId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // UserPermissions
            modelBuilder.Entity<UserPermission>(e =>
            {
                e.ToTable("UserPermissions");
                e.HasKey(x => new { x.UserId, x.PermissionId });

                e.HasOne(x => x.User)
                 .WithMany(x => x.UserPermissions)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Permission)
                 .WithMany(x => x.UserPermissions)
                 .HasForeignKey(x => x.PermissionId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasIndex(x => x.PermissionId);
                e.HasIndex(x => x.ExpiresAt);
            });

            // Update Customer to support new FKs
            modelBuilder.Entity<Customer>()
                .HasIndex(x => x.RoleId);

            modelBuilder.Entity<Customer>()
                .HasIndex(x => x.GroupId);
        }
    }
}
