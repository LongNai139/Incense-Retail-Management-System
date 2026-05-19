using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.BusinessLayers.Services
{
    public class VoucherApplyResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public decimal DiscountAmount { get; set; }
        public Voucher? Voucher { get; set; }
    }
}
