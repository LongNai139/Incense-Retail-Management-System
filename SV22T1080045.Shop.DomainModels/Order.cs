using System.ComponentModel.DataAnnotations.Schema;

namespace SV22T1080045.Shop.DomainModels
{
    [Table("Orders")]
    public class Order : _BaseEntity
    {
        public int CustomerId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; } = 0;

        // Trạng thái: 1 = Mới đặt, 2 = Đã duyệt, 3 = Đang giao, 4 = Hoàn thành, -1 = Hủy
        public int Status { get; set; } = 1;

        // Thông tin giao hàng (có thể khác với địa chỉ mặc định của khách)
        public string ShippingName { get; set; } = "";
        public string ShippingPhone { get; set; } = "";
        public string ShippingAddress { get; set; } = "";
        public string? Note { get; set; }

        // Thanh toán: 1=COD, 2=VNPay, 3=MoMo. PaymentStatus: 0=Chưa TT, 1=Đã TT, 2=Thất bại.
        public int PaymentMethod { get; set; } = 1;
        public int PaymentStatus { get; set; } = 0;
        public string? PaymentTransactionNo { get; set; }
        public string? PaymentBankCode { get; set; }
        public string? PaymentResponseCode { get; set; }
        public DateTime? PaidAt { get; set; }

        // ── Voucher ──────────────────────────────────────────────────────────
        /// <summary>Id của voucher được áp dụng (null nếu không dùng)</summary>
        public int? VoucherId { get; set; }

        /// <summary>Lưu lại mã voucher để hiển thị kể cả khi voucher bị xoá</summary>
        public string? VoucherCode { get; set; }

        /// <summary>Số tiền được giảm (tính tại thời điểm đặt hàng)</summary>
        public decimal DiscountAmount { get; set; } = 0;

        /// <summary>Tổng tiền sau khi đã trừ giảm giá (= TotalAmount - DiscountAmount)</summary>
        public decimal FinalAmount { get; set; } = 0;
        // ─────────────────────────────────────────────────────────────────────

        // Quan hệ 1-N: Một đơn hàng có nhiều chi tiết
        public virtual List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
