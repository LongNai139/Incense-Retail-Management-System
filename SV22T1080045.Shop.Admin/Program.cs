using Microsoft.AspNetCore.Authentication.Cookies;
using SV22T1080045.Shop.BusinessLayers;
using SV22T1080045.Shop.DataLayers;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // --- 1. Cấu hình MVC ---
        builder.Services.AddControllersWithViews();
        builder.Services.AddHttpContextAccessor(); // Để truy cập Session/Cookie ở tầng Service nếu cần

        // --- 2. Cấu hình Session (Cho Giỏ hàng) ---
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30); // Session tồn tại 30 phút
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        // --- 3. Cấu hình Authentication (Dùng Cookie thay vì Identity) ---
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "SV22T1080045_Shop_Auth"; // Tên cookie
                options.LoginPath = "/Account/Login";           // Đường dẫn khi chưa đăng nhập
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromDays(30); // Duy trì đăng nhập 30 ngày
            });

        // --- 4. Lấy chuỗi kết nối từ appsettings.json ---
        // Đảm bảo trong appsettings.json tên là "ShopDB" (hoặc tên bạn đặt)
        string connectionString = builder.Configuration.GetConnectionString("ShopDB");


        // --- 5. Đăng ký Dependency Injection (DI) cho DAL & Service ---

        // Đăng ký DAL (Phải truyền connectionString vào constructor)
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSession();

        builder.Services.AddScoped<AccountDAL>(provider => new AccountDAL(connectionString));
        builder.Services.AddScoped<ProductDAL>(provider => new ProductDAL(connectionString));
        builder.Services.AddScoped<OrderDAL>(provider => new OrderDAL(connectionString));


        // Đăng ký Service
        builder.Services.AddScoped<ProductService>();
        builder.Services.AddScoped<OrderService>();
        builder.Services.AddScoped<AccountService>();
        builder.Services.AddScoped<CartService>();


        var app = builder.Build();

        // --- 6. Cấu hình Pipeline ---
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseSession(); // Phải đặt trước Authentication
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}