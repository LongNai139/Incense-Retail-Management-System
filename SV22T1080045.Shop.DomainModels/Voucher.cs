using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV22T1080045.Shop.DomainModels
{
    public enum VoucherType { Percent = 1, FixedAmount = 2 }

    public class Voucher : _BaseEntity
    {
        public string Code { get; set; } = "";
        public string Description { get; set; } = "";
        public VoucherType DiscountType { get; set; } = VoucherType.Percent;

        /// <summary>Giá trị giảm: % hoặc số tiền cố định (VND)</summary>
        public decimal DiscountValue { get; set; }

        /// <summary>Giảm tối đa bao nhiêu (áp dụng khi DiscountType = Percent)</summary>
        public decimal? MaxDiscount { get; set; }

        /// <summary>Đơn hàng tối thiểu để áp dụng</summary>
        public decimal MinOrderAmount { get; set; } = 0;

        public DateTime? ExpiresAt { get; set; }
        public int? MaxUsage { get; set; }
        public int UsedCount { get; set; } = 0;
        public bool IsActive { get; set; } = true;

        public bool IsValid(decimal orderAmount)
        {
            if (!IsActive) return false;
            if (ExpiresAt.HasValue && ExpiresAt.Value < DateTime.Now) return false;
            if (MaxUsage.HasValue && UsedCount >= MaxUsage.Value) return false;
            if (orderAmount < MinOrderAmount) return false;
            return true;
        }

        public decimal Calculate(decimal orderAmount)
        {
            if (DiscountType == VoucherType.Percent)
            {
                var discount = orderAmount * DiscountValue / 100;
                return MaxDiscount.HasValue ? Math.Min(discount, MaxDiscount.Value) : discount;
            }
            return Math.Min(DiscountValue, orderAmount);
        }
    }
}
