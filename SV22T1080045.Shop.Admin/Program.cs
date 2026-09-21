using Microsoft.AspNetCore.Authentication.Cookies;
using SV22T1080045.Shop.BusinessLayers;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.DataLayers;
using SV22T1080045.Shop.DomainModels;
using SV22T1080045.Shop.Admin.Extensions.FileLogger;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Add file logger provider
builder.Logging.AddProvider(new FileLoggerProvider(Directory.GetCurrentDirectory()));

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

var connectionString = builder.Configuration.GetConnectionString("ShopConnectionString")!;

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.Name = "SV22T1080045_Shop_Admin_Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "SV22T1080045_Shop_Backoffice_Auth";
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

app.MapGet("/", (HttpContext context) =>
{
    if (context.User.IsInRole(CustomerRoles.Admin))
        return Results.Redirect("/Management");

    if (context.User.IsInRole(CustomerRoles.Staff))
        return Results.Redirect("/Staff");

    return Results.Redirect("/Account/Login");
});

app.MapGet("/Home", () => Results.Redirect(storefrontUrl.TrimEnd('/')));

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

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
