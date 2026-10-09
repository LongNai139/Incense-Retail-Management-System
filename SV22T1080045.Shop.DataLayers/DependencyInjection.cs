using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
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
            services.AddDbContext<ShopDbContext>(options =>
            {
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);
                });
                options.EnableDetailedErrors();
            });

            services.AddScoped<IProductDAL, ProductDAL>();
            services.AddScoped<ICategoryDAL, CategoryDAL>();
            services.AddScoped<IUnitDAL, UnitDAL>();
            services.AddScoped<ICustomerDAL, CustomerDAL>();
            services.AddScoped<ISystemDAL, SystemDAL>();
            services.AddScoped<IOrderDAL, OrderDAL>();
            services.AddScoped<IRevenueReportDAL, RevenueReportDAL>();
            services.AddScoped<IManagementDAL, ManagementDAL>();
            services.AddScoped<IStaffDAL, StaffDAL>();
            services.AddScoped<IVoucherDAL, VoucherDAL>();
            services.AddScoped<IGuestOrderDAL, GuestOrderDAL>();
            services.AddScoped<IPhoneOtpDAL, PhoneOtpDAL>();
            services.AddScoped<IFileStorageService, LocalFileStorageService>();
            services.AddScoped<IWarehouseDAL, WarehouseDAL>();

            return services;
        }
    }
}
