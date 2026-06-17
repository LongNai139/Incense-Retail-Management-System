using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.Models.Mappers;
using SV22T1080045.Shop.Models.ViewModels.Product;

namespace SV22T1080045.Shop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public IActionResult Index() => RedirectToAction("Index", "Management");

        public IActionResult Edit(int id)
        {
            if (id == 0)
            {
                ViewBag.Title = "Thêm mới sản phẩm";
                return View(new ProductEditViewModel());
            }

            ViewBag.Title = "Cập nhật sản phẩm";
            var product = _productService.GetProduct(id);
            if (product == null)
                return RedirectToAction("Index", "Management");

            return View(product.ToEditViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(ProductEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Edit", model);

            try
            {
                _productService.SaveProduct(model.ToSaveRequest());
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Edit", model);
            }

            return RedirectToAction("Index", "Management");
        }
    }
}
