using Microsoft.AspNetCore.Mvc;
using SV22T1080045.Shop.Models;
using SV22T1080045.Shop.BusinessLayers;
using SV22T1080045.Shop.DomainModels;
using SV22T1080045.Shop.Admin.AppCodes.Mappers;

namespace SV22T1080045.Shop.Admin.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public IActionResult Index(string searchValue = "", int CategoryId = 0, decimal OriginalPrice = 0, decimal PriceAfterDiscount = 0)
        {
            var products = _productService.ListProducts(searchValue, CategoryId, OriginalPrice, PriceAfterDiscount);
            return View(products);
        }

        // Handles both Create and Update operations.
        // id = 0: Create Mode
        // id > 0: Edit Mode
        public IActionResult Edit(int id)
        {
            // CASE 1: CREATE MODE
            if (id == 0)
            {
                ViewBag.Title = "Thêm mới sản phẩm"; // Keep UI text in Vietnamese for end-users
                // Return an empty model to render a blank form
                return View(new ProductEditModel());
            }

            // CASE 2: EDIT MODE
            ViewBag.Title = "Cập nhật sản phẩm";

            // Retrieve product data from the Service layer
            var product = _productService.GetProduct(id);
            if (product == null)
            {
                return RedirectToAction("Index");
            }

            // CRITICAL: Map DomainModel (Entity) to ViewModel (ProductEditModel)
            // This prepares data for the View presentation
            var model = product.ToEditModel();

            return View(model);
        }

        [HttpPost]
        public IActionResult Save(ProductEditModel model)
        {
            // 1. Validate input data based on Data Annotations
            if (!ModelState.IsValid)
            {
                ViewBag.Title = model.Id == 0 ? "Thêm mới sản phẩm" : "Cập nhật sản phẩm";
                return View("Edit", model); // Return to View with validation errors
            }

            // 2. Handle Image Upload (if a new file is provided)
            if (model.UploadPhoto != null)
            {
                // Generate a unique filename to prevent duplication conflicts
                var fileName = $"{DateTime.Now.Ticks}_{model.UploadPhoto.FileName}";

                // Define the physical storage path
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");
                var filePath = Path.Combine(folderPath, fileName);

                // Logic to delete the old image when updating (Clean up server storage)
                if (model.Id > 0 && !string.IsNullOrEmpty(model.ImageUrl))
                {
                    var oldFilePath = Path.Combine(folderPath, model.ImageUrl);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Save the new file to the stream
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.UploadPhoto.CopyTo(stream);
                }

                // Update the ImageUrl property to save into the Database
                model.ImageUrl = fileName;
            }

            // 3. Mapping: Convert ViewModel back to DomainModel
            var data = model.ToDomainModel();

            // 4. Call Service to persist data (Insert or Update)
            if (data.Id == 0)
            {
                _productService.AddProduct(data);
            }
            else
            {
                _productService.UpdateProduct(data);
            }

            return RedirectToAction("Index");
        }
    }
}