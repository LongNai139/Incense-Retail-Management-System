using SV22T1080045.Shop.BusinessLayers.Services;

namespace SV22T1080045.Shop.BusinessLayers.Interfaces
{
    public interface IVoucherService
    {
        VoucherApplyResult Apply(string code, decimal orderAmount);
        bool Use(string code);
    }
}
