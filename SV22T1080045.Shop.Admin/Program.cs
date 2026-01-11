using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SV22T1080045.Shop.BusinessLayers; // Nhớ thêm dòng này
using SV22T1080045.Shop.DataLayers;
// using SV22T1080045.Shop.App.Models; // Nếu cần

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews();
        builder.Services.AddHttpContextAccessor(); 

        string connectionString = builder.Configuration.GetConnectionString("ShopConnectionString");
        builder.Services.AddDbContext<ShopDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        // --- 4. CẤU HÌNH AUTHENTICATION ---
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "SV22T1080045_Shop_Auth";
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromDays(30);
            });

        // --- 5. ĐĂNG KÝ DI (DEPENDENCY INJECTION) ---

        // SỬA LỖI TẠI ĐÂY: Phải truyền connectionString vào Constructor
        builder.Services.AddScoped<IProductDAL, ProductDAL>();

        // B. Business Logic Layer (BLL) - QUAN TRỌNG
        // Phải đăng ký cái này thì Controller mới chạy được
        builder.Services.AddScoped<IProductService, ProductService>();


        // --- BUILD APP ---
        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseSession();
        app.UseAuthentication();
        app.UseAuthorization(); 

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}