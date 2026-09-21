using SV22T1080045.Shop.Models.ViewModels.Product;

namespace SV22T1080045.Shop.Models.ViewModels.Management
{
    public class ProductManagementViewModel
    {
        public List<ManagementCategoryOptionViewModel> Categories { get; set; } = new();
        public List<ManagementUnitOptionViewModel> Units { get; set; } = new();
        public List<ManagementProductRowViewModel> Products { get; set; } = new();
        public ProductEditViewModel ProductForm { get; set; } = new();

        public string SearchValue { get; set; } = "";
        public int? CategoryId { get; set; }
        public string ProductStatus { get; set; } = "";
        public int ProductPage { get; set; } = 1;
        public int ProductPageSize { get; set; } = 10;
        public int ProductTotalCount { get; set; }
        public int ProductTotalPages { get; set; } = 1;
        public int? LastSavedProductId { get; set; }
        public string? LastSavedAction { get; set; }
    }
}