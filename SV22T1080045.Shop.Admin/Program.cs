using Microsoft.AspNetCore.Authentication.Cookies;
using SV22T1080045.Shop.BusinessLayers;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.DataLayers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

var connectionString = builder.Configuration.GetConnectionString("ShopConnectionString")!;

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "SV22T1080045_Shop_Backoffice_Auth";
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

var storefrontUrl = app.Configuration["AppHosts:StorefrontUrl"] ?? "https://localhost:7126";

app.MapGet("/", () => Results.Redirect("/Management"));
app.MapGet("/Home", () => Results.Redirect($"{storefrontUrl.TrimEnd('/')}"));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Management}/{action=Index}/{id?}");

app.Run();

static void SeedStaffAccounts(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    try
    {
        scope.ServiceProvider.GetRequiredService<IStartupSeedService>().SeedStaffAccounts();
    }
    catch
    {
        // Database may be offline during startup.
    }
}
