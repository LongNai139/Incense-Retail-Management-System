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

        // Quan hệ 1-N: Một đơn hàng có nhiều chi tiết
        // Lưu ý: virtual để dùng Lazy Loading nếu cần
        public virtual List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}