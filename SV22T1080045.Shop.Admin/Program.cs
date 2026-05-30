using Microsoft.AspNetCore.Authentication.Cookies;
using SV22T1080045.Shop.Admin.Services;
using SV22T1080045.Shop.BusinessLayers;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.DataLayers;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews();
        builder.Services.AddHttpContextAccessor();
        builder.Services.Configure<VnPayOptions>(builder.Configuration.GetSection("VnPay"));
        builder.Services.AddScoped<IVnPayService, VnPayService>();

        string connectionString = builder.Configuration
            .GetConnectionString("ShopConnectionString")!;

        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "SV22T1080045_Shop_Auth";
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromDays(30);
            });

        builder.Services.AddDataLayers(connectionString);
        builder.Services.AddBusinessLayers();

        var app = builder.Build();

        SeedStaffAccounts(app);

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
            name: "tracuu",
            pattern: "tra-cuu-don-hang",
            defaults: new { controller = "OrderLookup", action = "Index" });

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }

    private static void SeedStaffAccounts(WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        try
        {
            scope.ServiceProvider.GetRequiredService<IStartupSeedService>().SeedStaffAccounts();
        }
        catch
        {
            // The app can still start when the database is not available; pages will surface DB errors normally.
        }
    }
}
