using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SV22T1080045.Shop.BusinessLayers;
using SV22T1080045.Shop.DataLayers;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews();
        builder.Services.AddHttpContextAccessor();

        // ── Connection String ───────────────────────────────────────────────
        string connectionString = builder.Configuration
            .GetConnectionString("ShopConnectionString")!;

        // ── EF Core (chỉ dùng cho Products, và Migration) ──────────────────
        builder.Services.AddDbContext<ShopDbContext>(options =>
            options.UseSqlServer(connectionString));

        // ── Session ─────────────────────────────────────────────────────────
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        // ── Authentication ──────────────────────────────────────────────────
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "SV22T1080045_Shop_Auth";
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromDays(30);
            });

        // ── DAL (Dapper) ────────────────────────────────────────────────────
        builder.Services.AddScoped<IProductDAL, ProductDAL>();
        builder.Services.AddScoped<ICategoryDAL, CategoryDAL>();
        builder.Services.AddScoped<ICustomerDAL, CustomerDAL>();
        // Các DAL dùng Dapper cần connectionString trực tiếp
        builder.Services.AddScoped(_ => new OrderDAL(connectionString));
        builder.Services.AddScoped(_ => new VoucherDAL(connectionString));
        builder.Services.AddScoped<IGuestOrderDAL>(_ => new GuestOrderDAL(connectionString));
        builder.Services.AddScoped<IPhoneOtpDAL>(_ => new PhoneOtpDAL(connectionString));

        // ── Business Services ───────────────────────────────────────────────
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<ICartService, CartService>();
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddScoped<IGuestOrderService, GuestOrderService>();
        builder.Services.AddScoped<IVoucherService, VoucherService>();
        builder.Services.AddScoped<IOtpService, OtpService>();
        builder.Services.AddScoped<AccountService>();
        builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();

        // ── SMS Service (dev: FakeSms | prod: đổi thành EsmsSmsService) ────
        builder.Services.AddSingleton<ISmsService, FakeSmsService>();

        // ── BUILD ───────────────────────────────────────────────────────────
        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseSession();           // phải trước UseAuthentication
        app.UseAuthentication();
        app.UseAuthorization();

        // Route cho trang tra cứu đơn hàng (URL đẹp)
        app.MapControllerRoute(
            name: "tracuu",
            pattern: "tra-cuu-don-hang",
            defaults: new { controller = "OrderLookup", action = "Index" });

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}