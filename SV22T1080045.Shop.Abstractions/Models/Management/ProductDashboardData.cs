using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.Abstractions.Models.Management;

public class ProductDashboardData
{
    public List<Product> BestSellers { get; set; } = new();
    public List<Product> LowStockProducts { get; set; } = new();
    public int OutOfStockCount { get; set; }
    public decimal CategoryRevenueTotal { get; set; }
    public List<ProductCategorySalesData> CategorySales { get; set; } = new();
}

public class ProductCategorySalesData
{
    public int CategoryId { get; set; }
    public int Quantity { get; set; }
    public decimal Revenue { get; set; }
    public decimal RevenueShare { get; set; }
}
