using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV22T1080045.Shop.DomainModels
{
    /// <summary>
    /// Lưu liên kết giữa số điện thoại (đã hash) và đơn hàng của khách vãng lai.
    /// Khách không cần tài khoản — chỉ cần SĐT để tra cứu lại đơn hàng.
    /// </summary>
    public class GuestOrder : _BaseEntity
    {
        /// <summary>SHA-256 của số điện thoại — không lưu SĐT gốc</summary>
        public string PhoneHash { get; set; } = "";

        /// <summary>4 số cuối SĐT — hiển thị dạng ****1234</summary>
        public string PhoneLastFour { get; set; } = "";

        /// <summary>FK đến bảng Orders (OrderID hiện tại của bạn)</summary>
        public int OrderId { get; set; }

        /// <summary>Nếu khách tạo tài khoản sau → điền CustomerId vào đây</summary>
        public int? ConvertedCustomerId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public Order? Order { get; set; }
        public Customer? ConvertedCustomer { get; set; }
    }
}
