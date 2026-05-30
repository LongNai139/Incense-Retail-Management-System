using Microsoft.AspNetCore.Mvc;
using SV22T1080045.Shop.BusinessLayers.Interfaces;

namespace SV22T1080045.Shop.ViewComponents
{
    public class CartBadgeViewComponent : ViewComponent
    {
        private readonly ICartService _cartService;

        public CartBadgeViewComponent(ICartService cartService)
        {
            _cartService = cartService;
        }

        public IViewComponentResult Invoke()
        {
            return View(_cartService.Count());
        }
    }
}
