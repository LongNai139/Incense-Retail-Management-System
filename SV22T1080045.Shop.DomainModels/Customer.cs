using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV22T1080045.Shop.DomainModels
{
    [Table("Customers")]
    public class Customer : _BaseEntity
    {
        public string CustomerName { get; set; } = "";
        public string Phone { get; set; } = ""; // Dùng làm định danh chính
        public string? Address { get; set; }

        // Cho phép null để khách vãng lai không cần tài khoản
        public string? Email { get; set; }
        public string? Password { get; set; }

        // Vai trò: "Admin" hoặc "Customer" (mặc định là Customer)
        public string Role { get; set; } = "Customer";
    }
}
