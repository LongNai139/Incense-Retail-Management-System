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
                options.UseSqlServer(connectionString));

            services.AddScoped<IProductDAL, ProductDAL>();
            services.AddScoped<ICategoryDAL, CategoryDAL>();
            services.AddScoped<IUnitDAL, UnitDAL>();
            services.AddScoped<ICustomerDAL, CustomerDAL>();
            services.AddScoped<IOrderDAL>(_ => new OrderDAL(connectionString));
            services.AddScoped<IRevenueReportDAL>(_ => new RevenueReportDAL(connectionString));
            services.AddScoped<IManagementDAL>(_ => new ManagementDAL(connectionString));
            services.AddScoped<IStaffDAL>(_ => new StaffDAL(connectionString));
            services.AddScoped<IVoucherDAL>(_ => new VoucherDAL(connectionString));
            services.AddScoped<IGuestOrderDAL>(_ => new GuestOrderDAL(connectionString));
            services.AddScoped<IPhoneOtpDAL>(_ => new PhoneOtpDAL(connectionString));
            services.AddScoped<IFileStorageService, LocalFileStorageService>();

            return services;
        }
    }
}
