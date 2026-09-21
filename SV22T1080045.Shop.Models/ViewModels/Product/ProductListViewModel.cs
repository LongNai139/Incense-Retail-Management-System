using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV22T1080045.Shop.Models.ViewModels.Product
{
    /// <summary>
    /// ViewModel chứa toàn bộ dữ liệu cần thiết cho trang danh sách sản phẩm.
    /// </summary>
    public class ProductListViewModel
    {
        // ── Danh sách sản phẩm hiện tại (đã phân trang) ──────────────────
        public List<ProductCardViewModel> Products { get; set; } = new();

        // ── Sidebar: danh mục & bộ lọc ───────────────────────────────────
        public List<CategoryFilterItem> Categories { get; set; } = new();

        // ── Tham số tìm kiếm / lọc (để giữ lại giá trị trên form) ───────
        public string? SearchValue { get; set; }
        public int? CategoryId { get; set; }
        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }
        public string? Origin { get; set; }
        public string? UsageTag { get; set; }
        public int? MinRating { get; set; }
        public string SortBy { get; set; } = "default";  // default | bestseller | newest | price-asc | price-desc | rating

        // ── Phân trang ────────────────────────────────────────────────────
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        // ── Danh sách xuất xứ (populate từ DB) ───────────────────────────
        public List<string> OriginOptions { get; set; } = new();
        public Dictionary<string, int> OriginCounts { get; set; } = new();
    }
}
