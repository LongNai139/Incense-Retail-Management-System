using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SV22T1080045.Shop.Payments.MoMo;
using SV22T1080045.Shop.Payments.VietQr;
using SV22T1080045.Shop.Payments.VnPay;

namespace SV22T1080045.Shop.Payments
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddShopPayments(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<VnPayOptions>(configuration.GetSection("VnPay"));
            services.Configure<MoMoOptions>(configuration.GetSection("MoMo"));
            services.Configure<VietQrOptions>(configuration.GetSection("VietQr"));

            services.AddHttpClient(nameof(MoMoService));
            services.AddScoped<IVnPayService, VnPayService>();
            services.AddScoped<IMoMoService, MoMoService>();
            services.AddScoped<IVietQrService, VietQrService>();

            return services;
        }
    }
}
