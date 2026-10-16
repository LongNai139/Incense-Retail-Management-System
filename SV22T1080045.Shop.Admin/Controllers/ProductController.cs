using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1080045.Shop.Admin.Extensions.Authorizations;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.DomainModels;
using SV22T1080045.Shop.Models.Mappers;
using SV22T1080045.Shop.Models.ViewModels.Product;

namespace SV22T1080045.Shop.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [Permission(PermissionConstants.ViewProduct)]
        public IActionResult Index() => RedirectToAction("Index", "Management");

        public IActionResult Edit(int id)
        {
            var requiredPermission = id == 0 ? PermissionConstants.AddProduct : PermissionConstants.EditProduct;
            if (!User.HasAnyPermission(requiredPermission, PermissionConstants.ManageSystem))
                return Forbid();

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
            var requiredPermission = model.Id > 0 ? PermissionConstants.EditProduct : PermissionConstants.AddProduct;
            if (!User.HasAnyPermission(requiredPermission, PermissionConstants.ManageSystem))
                return Forbid();

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