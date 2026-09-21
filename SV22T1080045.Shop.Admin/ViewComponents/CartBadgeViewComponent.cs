using Microsoft.AspNetCore.Mvc;
using SV22T1080045.Shop.BusinessLayers.Interfaces;

namespace SV22T1080045.Shop.Admin.ViewComponents
{
    public class CartBadgeViewComponent : ViewComponent
    {
        private readonly ICartService _cartService;
        private readonly IConfiguration _configuration;

        public CartBadgeViewComponent(ICartService cartService, IConfiguration configuration)
        {
            _cartService = cartService;
            _configuration = configuration;
        }

        public IViewComponentResult Invoke()
        {
            ViewBag.StorefrontUrl = _configuration["AppHosts:StorefrontUrl"] ?? "https://localhost:7126";
            return View(_cartService.Count());
        }
    }
}
