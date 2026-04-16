using SV22T1080045.Shop.DataLayers;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.BusinessLayers
{
    // ── Result DTO ────────────────────────────────────────────────────────────
    public class VoucherApplyResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public decimal DiscountAmount { get; set; }
        public Voucher? Voucher { get; set; }
    }

    // ── Interface ─────────────────────────────────────────────────────────────
    public interface IVoucherService
    {
        VoucherApplyResult Apply(string code, decimal orderAmount);
        bool Use(string code);
    }

    // ── Implementation ────────────────────────────────────────────────────────
    public class VoucherService : IVoucherService
    {
        private readonly VoucherDAL _dal;

        public VoucherService(VoucherDAL dal) => _dal = dal;

        public VoucherApplyResult Apply(string code, decimal orderAmount)
        {
            if (string.IsNullOrWhiteSpace(code))
                return new VoucherApplyResult { Message = "Vui lòng nhập mã giảm giá." };

            var voucher = _dal.GetByCode(code);
            if (voucher == null)
                return new VoucherApplyResult { Message = "Mã giảm giá không tồn tại." };

            if (!voucher.IsValid(orderAmount))
            {
                if (voucher.ExpiresAt.HasValue && voucher.ExpiresAt.Value < DateTime.Now)
                    return new VoucherApplyResult { Message = "Mã giảm giá đã hết hạn." };
                if (voucher.MaxUsage.HasValue && voucher.UsedCount >= voucher.MaxUsage.Value)
                    return new VoucherApplyResult { Message = "Mã giảm giá đã được dùng hết." };
                if (orderAmount < voucher.MinOrderAmount)
                    return new VoucherApplyResult
                    {
                        Message = $"Đơn hàng tối thiểu {voucher.MinOrderAmount:N0}đ để dùng mã này."
                    };
                return new VoucherApplyResult { Message = "Mã giảm giá không hợp lệ." };
            }

            var discount = voucher.Calculate(orderAmount);
            return new VoucherApplyResult
            {
                Success = true,
                Message = $"Giảm {discount:N0}đ",
                DiscountAmount = discount,
                Voucher = voucher
            };
        }

        public bool Use(string code) => _dal.Use(code);
    }
}