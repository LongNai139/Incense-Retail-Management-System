using SV22T1080045.Shop.Abstractions.Contracts.Products;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.Abstractions.Models;
using SV22T1080045.Shop.BusinessLayers.Helpers;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.BusinessLayers.Services
{
    public class ProductService : IProductService
    {
        private const string ProductImageFolder = "images/products";

        private readonly IProductDAL _productDal;
        private readonly IFileStorageService _fileStorage;

        public ProductService(IProductDAL productDal, IFileStorageService fileStorage)
        {
            _productDal = productDal;
            _fileStorage = fileStorage;
        }

        public ProductListResult ListProductsFiltered(ProductQuery query)
        {
            query.Page = Math.Max(1, query.Page);
            query.PageSize = Math.Max(1, query.PageSize);
            query.SearchValue = query.SearchValue?.Trim();
            query.Origin = query.Origin?.Trim();
            query.UsageTag = query.UsageTag?.Trim();

            if (query.PriceMin.HasValue && query.PriceMax.HasValue && query.PriceMin > query.PriceMax)
                (query.PriceMin, query.PriceMax) = (query.PriceMax, query.PriceMin);

            return _productDal.Query(query);
        }

        public List<Product> ListProducts(string? searchValue = null, int take = 0, string sortBy = "newest")
            => _productDal.List(searchValue, take, sortBy);

        public List<Product> ListRelatedProducts(int productId, int categoryId, int take = 0)
            => _productDal.ListByCategory(categoryId, productId, take);

        public Product? GetProduct(int id)
            => _productDal.GetById(id);

        public bool DeleteProduct(int id)
            => _productDal.Delete(id);

        public void SaveProduct(ProductSaveRequest request)
        {
            var productName = (request.ProductName ?? string.Empty).Trim();

            ValidateSaveRequest(request, productName);

            if (_productDal.IsNameExists(productName, request.Id))
                throw new InvalidOperationException("Tên sản phẩm đã tồn tại.");

            var existing = request.Id > 0 ? _productDal.GetById(request.Id) : null;
            if (request.Id > 0 && existing == null)
                throw new InvalidOperationException("Sản phẩm không tồn tại hoặc đã bị xóa.");

            var imageUrl = ResolveImageUrl(request.UploadPhoto, request.ExistingImageUrl, existing?.ImageUrl);
            var imageUrl2 = ResolveImageUrl(request.UploadPhoto2, request.ExistingImageUrl2, existing?.ImageUrl2);
            var imageUrl3 = ResolveImageUrl(request.UploadPhoto3, request.ExistingImageUrl3, existing?.ImageUrl3);
            var imageUrl4 = ResolveImageUrl(request.UploadPhoto4, request.ExistingImageUrl4, existing?.ImageUrl4);

            var product = new Product
            {
                Id = request.Id,
                ProductName = productName,
                UnitId = request.UnitId,
                CategoryId = request.CategoryId,
                OriginalPrice = request.OriginalPrice,
                PriceAfterDiscount = request.PriceAfterDiscount,
                BurningTime = request.BurningTime?.Trim(),
                Ingredient = request.Ingredient?.Trim(),
                Description = request.Description?.Trim(),
                Origin = request.Origin?.Trim(),
                AgeYear = request.AgeYear?.Trim(),
                OilContent = request.OilContent?.Trim(),
                Length = request.Length?.Trim(),
                Weight = request.Weight?.Trim(),
                UsageTags = request.UsageTags?.Trim(),
                Quantity = request.Quantity,
                SoldCount = request.SoldCount,
                Rating = request.Rating,
                ReviewCount = request.ReviewCount,
                ImageUrl = imageUrl,
                ImageUrl2 = imageUrl2,
                ImageUrl3 = imageUrl3,
                ImageUrl4 = imageUrl4,
                CreatedTime = existing?.CreatedTime ?? DateTime.Now,
                IsDeleted = existing?.IsDeleted ?? false
            };

            if (product.Id == 0)
                _productDal.Add(product);
            else
                _productDal.Update(product);
        }

        private void ValidateSaveRequest(ProductSaveRequest request, string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                throw new InvalidOperationException("Tên sản phẩm không được để trống.");

            if (request.CategoryId <= 0 || !_productDal.CategoryExists(request.CategoryId))
                throw new InvalidOperationException("Loại hàng không tồn tại.");

            if (request.UnitId <= 0 || !_productDal.UnitExists(request.UnitId))
                throw new InvalidOperationException("Đơn vị tính không tồn tại.");

            if (request.OriginalPrice < 0)
                throw new InvalidOperationException("Giá gốc không hợp lệ.");

            if (request.PriceAfterDiscount < 0)
                throw new InvalidOperationException("Giá bán không hợp lệ.");

            if (request.PriceAfterDiscount > request.OriginalPrice)
                throw new InvalidOperationException("Giá bán không được lớn hơn giá gốc.");

            ValidateNonNegative(request.Quantity, "Số lượng tồn không hợp lệ.");
            ValidateNonNegative(request.SoldCount, "Số lượng đã bán không hợp lệ.");
            ValidateNonNegative(request.ReviewCount, "Số lượng đánh giá không hợp lệ.");

            if (request.Rating is < 0 or > 5)
                throw new InvalidOperationException("Đánh giá chỉ được trong khoảng từ 0 đến 5.");

            ValidateImageUpload(request.UploadPhoto);
            ValidateImageUpload(request.UploadPhoto2);
            ValidateImageUpload(request.UploadPhoto3);
            ValidateImageUpload(request.UploadPhoto4);
        }

        private static void ValidateNonNegative(int? value, string errorMessage)
        {
            if (value.HasValue && value.Value < 0)
                throw new InvalidOperationException(errorMessage);
        }

        private static void ValidateImageUpload(FileUploadData? upload)
        {
            if (upload == null)
                return;

            if (!upload.HasContent)
                throw new InvalidOperationException("Tệp ảnh tải lên không hợp lệ.");

            if (string.IsNullOrWhiteSpace(upload.ContentType) ||
                !upload.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Chỉ chấp nhận tải lên tệp ảnh.");
            }
        }

        private string? ResolveImageUrl(FileUploadData? upload, string? existingImageUrl, string? currentImageUrl)
        {
            if (upload?.HasContent == true)
            {
                var storedFileName = _fileStorage.SaveFile(upload, ProductImageFolder);
                var newUrl = ProductImageUrlHelper.Normalize(storedFileName);

                var oldFileName = ProductImageUrlHelper.ExtractFileName(currentImageUrl);
                if (!string.IsNullOrWhiteSpace(oldFileName))
                    _fileStorage.DeleteFile(ProductImageFolder, oldFileName);

                return newUrl;
            }

            if (!string.IsNullOrWhiteSpace(existingImageUrl))
                return ProductImageUrlHelper.Normalize(existingImageUrl);

            return ProductImageUrlHelper.Normalize(currentImageUrl);
        }
    }
}
