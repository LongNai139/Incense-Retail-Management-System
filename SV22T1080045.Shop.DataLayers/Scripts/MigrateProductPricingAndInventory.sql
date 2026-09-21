-- Backfill giá & tồn kho sau khi thêm ImportPrice, SalePrice, DiscountPercent, ProductInventories.
-- Chạy SAU khi đã apply migration EF AddProductPricingAndInventory.

-- 1) Giá bán niêm yết từ OriginalPrice (dữ liệu cũ)
UPDATE p
SET
    p.SalePrice = CASE WHEN p.SalePrice > 0 THEN p.SalePrice ELSE p.OriginalPrice END,
    p.DiscountPercent = CASE
        WHEN p.DiscountPercent > 0 THEN p.DiscountPercent
        WHEN p.OriginalPrice > 0 AND p.PriceAfterDiscount > 0 AND p.PriceAfterDiscount < p.OriginalPrice
            THEN ROUND((1 - (p.PriceAfterDiscount / p.OriginalPrice)) * 100, 2)
        ELSE 0
    END
FROM dbo.Products p
WHERE p.IsDeleted = 0;

-- 2) Đồng bộ PriceAfterDiscount theo % (nếu có giảm giá)
UPDATE p
SET p.PriceAfterDiscount = ROUND(p.SalePrice * (100 - p.DiscountPercent) / 100, 0)
FROM dbo.Products p
WHERE p.IsDeleted = 0
  AND p.SalePrice > 0
  AND p.DiscountPercent > 0;

-- 3) Tồn kho tách bảng ProductInventories
MERGE dbo.ProductInventories AS target
USING (
    SELECT
        p.Id AS ProductId,
        ISNULL(p.Quantity, 0) AS Quantity,
        5 AS LowStockThreshold,
        SYSDATETIME() AS UpdatedTime
    FROM dbo.Products p
    WHERE p.IsDeleted = 0
) AS source
ON target.ProductId = source.ProductId
WHEN MATCHED THEN
    UPDATE SET
        target.Quantity = CASE WHEN target.Quantity = 0 THEN source.Quantity ELSE target.Quantity END,
        target.UpdatedTime = SYSDATETIME()
WHEN NOT MATCHED THEN
    INSERT (ProductId, Quantity, LowStockThreshold, UpdatedTime)
    VALUES (source.ProductId, source.Quantity, source.LowStockThreshold, source.UpdatedTime);
