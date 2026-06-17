using Microsoft.AspNetCore.Http;

namespace SV22T1080045.Shop.Payments.MoMo
{
    public interface IMoMoService
    {
        bool IsConfigured { get; }
        Task<MoMoPaymentResult> CreatePaymentAsync(
            int orderId,
            decimal amount,
            string orderInfo,
            string redirectUrl,
            string ipnUrl,
            CancellationToken cancellationToken = default);

        MoMoIpnResult ReadIpn(IQueryCollection query);
        MoMoIpnResult ReadReturn(IQueryCollection query);
    }
}
