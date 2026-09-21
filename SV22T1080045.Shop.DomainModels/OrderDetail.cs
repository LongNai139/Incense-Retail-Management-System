using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SV22T1080045.Shop.DomainModels
{
    [Table("OrderDetails")]
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; } // Khóa chính riêng cho dòng chi tiết

        public int OrderId { get; set; }
        public int ProductId { get; set; }

        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; } = 0; // Giá bán tại thời điểm mua

        [NotMapped]
        public string? ProductName { get; set; }

        [NotMapped]
        public string? Photo { get; set; }

        // Thuộc tính điều hướng (để dễ dàng lấy tên SP khi hiển thị đơn hàng)
        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
    }
}
