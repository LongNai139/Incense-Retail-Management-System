using Microsoft.AspNetCore.Http;
using SV22T1080045.Shop.Abstractions.Contracts.Products;
using SV22T1080045.Shop.Abstractions.Models;
using SV22T1080045.Shop.BusinessLayers.Helpers;
using SV22T1080045.Shop.DomainModels;
using SV22T1080045.Shop.Models.ViewModels.Product;

namespace SV22T1080045.Shop.Models.Mappers
{
    public static class ProductMappingExtensions
    {
        public static ProductCardViewModel ToCardViewModel(this Product source)
        {
            return new ProductCardViewModel
            {
                Id = source.Id,
                ProductName = source.ProductName,
                ImageUrl = ResolvePrimaryImageUrl(source),
                Origin = source.Origin,
                SalePrice = source.DisplaySalePrice,
                DiscountPercentValue = source.DiscountPercent,
                OriginalPrice = source.OriginalPrice,
                PriceAfterDiscount = source.DisplayPrice,
                Rating = source.Rating,
                ReviewCount = source.ReviewCount,
                SoldCount = source.SoldCount,
                Quantity = source.DisplayQuantity,
                LowStockThreshold = source.DisplayLowStockThreshold,
                UsageTags = source.UsageTags,
                CategoryName = source.Category?.CategoryName,
                CreatedTime = source.CreatedTime
            };
        }

        public static ProductDetailsViewModel ToDetailsViewModel(
            this Product source,
            IEnumerable<Product>? relatedProducts = null)
        {
            var galleryImageUrls = BuildGalleryImageUrls(source);

            return new ProductDetailsViewModel
            {
                Id = source.Id,
                ProductName = source.ProductName,
                Description = source.Description,
                Ingredient = source.Ingredient,
                BurningTime = source.BurningTime,
                Origin = source.Origin,
                AgeYear = source.AgeYear,
                OilContent = source.OilContent,
                Length = source.Length,
                Weight = source.Weight,
                Quantity = source.DisplayQuantity,
                LowStockThreshold = source.DisplayLowStockThreshold,
                SoldCount = source.SoldCount,
                Rating = source.Rating,
                ReviewCount = source.ReviewCount,
                SalePrice = source.DisplaySalePrice,
                DiscountPercentValue = source.DiscountPercent,
                OriginalPrice = source.OriginalPrice,
                PriceAfterDiscount = source.DisplayPrice,
                ImageUrl = ProductImageUrlHelper.Normalize(source.ImageUrl) ?? galleryImageUrls.FirstOrDefault(),
                UsageTags = ParseUsageTags(source.UsageTags),
                CategoryName = source.Category?.CategoryName,
                UnitName = source.Unit?.UnitName,
                GalleryImageUrls = galleryImageUrls,
                RelatedProducts = relatedProducts?
                    .Select(p => p.ToCardViewModel())
                    .ToList() ?? new List<ProductCardViewModel>()
            };
        }

        public static ProductEditViewModel ToEditViewModel(this Product source)
        {
            return new ProductEditViewModel
            {
                Id = source.Id,
                ProductName = source.ProductName,
                UnitId = source.UnitId,
                CategoryId = source.CategoryId,
                ImportPrice = source.ImportPrice,
                SalePrice = source.DisplaySalePrice,
                DiscountPercent = source.DiscountPercent,
                OriginalPrice = source.DisplaySalePrice,
                PriceAfterDiscount = source.DisplayPrice,
                BurningTime = source.BurningTime,
                Ingredient = source.Ingredient,
                Description = source.Description,
                Origin = source.Origin,
                AgeYear = source.AgeYear,
                OilContent = source.OilContent,
                Length = source.Length,
                Weight = source.Weight,
                UsageTags = source.UsageTags,
                Quantity = source.DisplayQuantity,
                LowStockThreshold = source.DisplayLowStockThreshold,
                SoldCount = source.SoldCount,
                Rating = source.Rating,
                ReviewCount = source.ReviewCount,
                ImageUrl = ProductImageUrlHelper.Normalize(source.ImageUrl),
                ImageUrl2 = ProductImageUrlHelper.Normalize(source.ImageUrl2),
                ImageUrl3 = ProductImageUrlHelper.Normalize(source.ImageUrl3),
                ImageUrl4 = ProductImageUrlHelper.Normalize(source.ImageUrl4)
            };
        }

        public static ProductSaveRequest ToSaveRequest(this ProductEditViewModel source)
        {
            return new ProductSaveRequest
            {
                Id = source.Id,
                ProductName = source.ProductName,
                UnitId = source.UnitId,
                CategoryId = source.CategoryId,
                ImportPrice = source.ImportPrice,
                SalePrice = source.SalePrice,
                DiscountPercent = source.DiscountPercent,
                OriginalPrice = source.OriginalPrice,
                PriceAfterDiscount = source.PriceAfterDiscount,
                BurningTime = source.BurningTime,
                Ingredient = source.Ingredient,
                Description = source.Description,
                Origin = source.Origin,
                AgeYear = source.AgeYear,
                OilContent = source.OilContent,
                Length = source.Length,
                Weight = source.Weight,
                UsageTags = source.UsageTags,
                Quantity = source.Quantity,
                LowStockThreshold = source.LowStockThreshold,
                SoldCount = source.SoldCount,
                Rating = source.Rating,
                ReviewCount = source.ReviewCount,
                ExistingImageUrl = source.ImageUrl,
                ExistingImageUrl2 = source.ImageUrl2,
                ExistingImageUrl3 = source.ImageUrl3,
                ExistingImageUrl4 = source.ImageUrl4,
                UploadPhoto = ToFileUploadData(source.UploadPhoto),
                UploadPhoto2 = ToFileUploadData(source.UploadPhoto2),
                UploadPhoto3 = ToFileUploadData(source.UploadPhoto3),
                UploadPhoto4 = ToFileUploadData(source.UploadPhoto4)
            };
        }

        public static List<string> ParseUsageTags(string? usageTags)
        {
            if (string.IsNullOrWhiteSpace(usageTags))
                return new List<string>();

            return usageTags
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();
        }

        private static string? ResolvePrimaryImageUrl(Product source)
        {
            return BuildGalleryImageUrls(source).FirstOrDefault();
        }

        private static List<string> BuildGalleryImageUrls(Product source)
        {
            return new[] { source.ImageUrl, source.ImageUrl2, source.ImageUrl3, source.ImageUrl4 }
                .Select(ProductImageUrlHelper.Normalize)
                .Where(imageUrl => !string.IsNullOrWhiteSpace(imageUrl))
                .Distinct()
                .Cast<string>()
                .ToList();
        }

        private static FileUploadData? ToFileUploadData(IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return null;

            using var memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);

            return new FileUploadData
            {
                FileName = Path.GetFileName(file.FileName),
                ContentType = file.ContentType ?? string.Empty,
                Content = memoryStream.ToArray()
            };
        }
    }
}
