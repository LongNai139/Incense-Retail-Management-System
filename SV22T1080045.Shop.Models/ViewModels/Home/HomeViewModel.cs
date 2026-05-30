using SV22T1080045.Shop.Models.ViewModels.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV22T1080045.Shop.Models.ViewModels.Home
{
    public class HomeViewModel
    {
        public List<ProductCardViewModel> FeaturedProducts { get; set; } = new();
        public List<ProductCardViewModel> LatestProducts { get; set; } = new();
        public List<CategoryViewModel> Categories { get; set; } = new();
    }
}
