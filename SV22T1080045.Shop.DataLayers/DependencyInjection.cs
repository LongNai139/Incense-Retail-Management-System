using Microsoft.Extensions.DependencyInjection;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.DataLayers.Implements;

namespace SV22T1080045.Shop.DataLayers
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataLayers(
            this IServiceCollection services,
            string connectionString)
        {
            services.AddScoped<IProductDAL, ProductDAL>();
            services.AddScoped<ICategoryDAL, CategoryDAL>();
            services.AddScoped<ICustomerDAL, CustomerDAL>();
            services.AddScoped<IOrderDAL>(_ => new OrderDAL(connectionString));
            services.AddScoped<IVoucherDAL>(_ => new VoucherDAL(connectionString));
            services.AddScoped<IGuestOrderDAL>(_ => new GuestOrderDAL(connectionString));
            services.AddScoped<IPhoneOtpDAL>(_ => new PhoneOtpDAL(connectionString));
            services.AddScoped<IFileStorageService, LocalFileStorageService>();

            return services;
        }
    }
}
