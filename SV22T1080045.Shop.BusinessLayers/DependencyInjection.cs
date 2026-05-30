using Microsoft.Extensions.DependencyInjection;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.BusinessLayers.Services;

namespace SV22T1080045.Shop.BusinessLayers
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessLayers(this IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IUnitService, UnitService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IVoucherService, VoucherService>();
            services.AddScoped<IOrderService>(sp => new OrderService(
                sp.GetRequiredService<IOrderDAL>(),
                sp.GetRequiredService<IVoucherService>()
            ));

            services.AddScoped<IRevenueReportService, RevenueReportService>();
            services.AddScoped<IManagementService, ManagementService>();
            services.AddScoped<IStaffService, StaffService>();
            services.AddScoped<IGuestOrderService, GuestOrderService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IPasswordHasherService, PasswordHasherService>();
            services.AddScoped<IStartupSeedService, StartupSeedService>();
            services.AddSingleton<ISmsService, FakeSmsService>();

            return services;
        }
    }
}
