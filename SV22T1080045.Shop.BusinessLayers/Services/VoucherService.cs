using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.BusinessLayers.Interfaces;

namespace SV22T1080045.Shop.BusinessLayers.Services
{
    public class VoucherService : IVoucherService
    {
        private readonly IVoucherDAL _voucherDAL;

        public VoucherService(IVoucherDAL voucherDAL)
        {
            _voucherDAL = voucherDAL;
        }

        public VoucherApplyResult Apply(string code, decimal orderAmount)
        {
            if (string.IsNullOrWhiteSpace(code))
                return new VoucherApplyResult { Message = "Vui lòng nhập mã giảm giá." };

            var voucher = _voucherDAL.GetByCode(code);
            if (voucher == null)
                return new VoucherApplyResult { Message = "Mã giảm giá không tồn tại." };

            if (!voucher.IsValid(orderAmount))
            {
                if (voucher.ExpiresAt.HasValue && voucher.ExpiresAt.Value < DateTime.Now)
                    return new VoucherApplyResult { Message = "Mã giảm giá đã hết hạn." };
                if (voucher.MaxUsage.HasValue && voucher.UsedCount >= voucher.MaxUsage.Value)
                    return new VoucherApplyResult { Message = "Mã giảm giá đã được dùng hết." };
                if (orderAmount < voucher.MinOrderAmount)
                    return new VoucherApplyResult { Message = $"Đơn hàng tối thiểu {voucher.MinOrderAmount:N0}đ để dùng mã này." };

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

        public bool Use(string code) => _voucherDAL.Use(code);
    }
}
