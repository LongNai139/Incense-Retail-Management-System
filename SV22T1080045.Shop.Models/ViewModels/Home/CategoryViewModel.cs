using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV22T1080045.Shop.Models.ViewModels.Home
{
    /// <summary>
    /// ViewModel nhẹ cho danh mục (dùng ở trang chủ và sidebar).
    /// </summary>
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = "";
    }
}
