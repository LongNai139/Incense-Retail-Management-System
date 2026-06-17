using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV22T1080045.Shop.Models.ViewModels.Product
{
    /// <summary>
    /// Dùng để hiển thị danh mục ở sidebar kèm số lượng sản phẩm.
    /// </summary>
    public class CategoryFilterItem
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = "";
        public int ProductCount { get; set; }
    }
}
