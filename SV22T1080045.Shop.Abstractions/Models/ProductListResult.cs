using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.Abstractions.Models
{
    public class ProductListResult
    {
        public List<Product> Products { get; set; } = new();
        public int TotalCount { get; set; }
        public List<string> OriginOptions { get; set; } = new();
        public Dictionary<string, int> OriginCounts { get; set; } = new();
        public Dictionary<int, int> CategoryCounts { get; set; } = new();
    }
}
