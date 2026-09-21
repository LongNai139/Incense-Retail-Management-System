using Microsoft.AspNetCore.Authentication.Cookies;
using SV22T1080045.Shop.BusinessLayers;
using SV22T1080045.Shop.DataLayers;
using SV22T1080045.Shop.Payments;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddShopPayments(builder.Configuration);

var connectionString = builder.Configuration.GetConnectionString("ShopConnectionString")!;

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.Name = "SV22T1080045_Shop_Web_Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "SV22T1080045_Shop_Customer_Auth";
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

builder.Services.AddDataLayers(connectionString);
builder.Services.AddBusinessLayers();

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
    name: "tracuu",
    pattern: "tra-cuu-don-hang",
    defaults: new { controller = "OrderLookup", action = "Index" });

app.MapControllerRoute(
    name: "myorders",
    pattern: "don-hang-cua-toi",
    defaults: new { controller = "MyOrders", action = "Index" });

app.MapControllerRoute(
    name: "orderTrack",
    pattern: "don-hang/theo-doi/{id:int}",
    defaults: new { controller = "OrderTracking", action = "Detail" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
