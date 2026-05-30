using Microsoft.AspNetCore.Http;

namespace SV22T1080045.Shop.Admin.Services
{
    public interface IVnPayService
    {
        bool IsConfigured { get; }
        string CreatePaymentUrl(int orderId, decimal amount, string orderInfo, string ipAddress, string returnUrl);
        VnPayReturnResult ReadReturn(IQueryCollection query);
    }
}
