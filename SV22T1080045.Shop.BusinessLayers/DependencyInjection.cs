using Microsoft.Extensions.DependencyInjection;
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
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IGuestOrderService, GuestOrderService>();
            services.AddScoped<IVoucherService, VoucherService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IPasswordHasherService, PasswordHasherService>();
            services.AddSingleton<ISmsService, FakeSmsService>();

            return services;
        }
    }
}
