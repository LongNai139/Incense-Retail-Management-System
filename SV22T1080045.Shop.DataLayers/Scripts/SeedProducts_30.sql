USE [ShopIncenseDB]
GO

SET IDENTITY_INSERT [dbo].[Categories] ON
GO

IF EXISTS (SELECT 1 FROM [dbo].[Categories] WHERE [Id] = 1)
UPDATE [dbo].[Categories] SET [CategoryName] = N'Trầm hương', [Description] = N'Sản phẩm trầm', [ImageUrl] = NULL, [IsDeleted] = 0 WHERE [Id] = 1
ELSE
INSERT [dbo].[Categories] ([Id], [CategoryName], [Description], [ImageUrl], [CreatedTime], [IsDeleted]) VALUES (1, N'Trầm hương', N'Sản phẩm trầm', NULL, SYSDATETIME(), 0)
GO
IF EXISTS (SELECT 1 FROM [dbo].[Categories] WHERE [Id] = 2)
UPDATE [dbo].[Categories] SET [CategoryName] = N'Nhan đốt', [Description] = N'Các loại nhang', [ImageUrl] = NULL, [IsDeleted] = 0 WHERE [Id] = 2
ELSE
INSERT [dbo].[Categories] ([Id], [CategoryName], [Description], [ImageUrl], [CreatedTime], [IsDeleted]) VALUES (2, N'Nhan đốt', N'Các loại nhang', NULL, SYSDATETIME(), 0)
GO
IF EXISTS (SELECT 1 FROM [dbo].[Categories] WHERE [Id] = 3)
UPDATE [dbo].[Categories] SET [CategoryName] = N'Phụ kiện', [Description] = N'Dụng cụ xông trầm', [ImageUrl] = NULL, [IsDeleted] = 0 WHERE [Id] = 3
ELSE
INSERT [dbo].[Categories] ([Id], [CategoryName], [Description], [ImageUrl], [CreatedTime], [IsDeleted]) VALUES (3, N'Phụ kiện', N'Dụng cụ xông trầm', NULL, SYSDATETIME(), 0)
GO

SET IDENTITY_INSERT [dbo].[Categories] OFF
GO

SET IDENTITY_INSERT [dbo].[Units] ON
GO

IF EXISTS (SELECT 1 FROM [dbo].[Units] WHERE [Id] = 1)
UPDATE [dbo].[Units] SET [UnitName] = N'Cái', [IsDeleted] = 0 WHERE [Id] = 1
ELSE
INSERT [dbo].[Units] ([Id], [UnitName], [CreatedTime], [IsDeleted]) VALUES (1, N'Cái', SYSDATETIME(), 0)
GO
IF EXISTS (SELECT 1 FROM [dbo].[Units] WHERE [Id] = 2)
UPDATE [dbo].[Units] SET [UnitName] = N'Vòng', [IsDeleted] = 0 WHERE [Id] = 2
ELSE
INSERT [dbo].[Units] ([Id], [UnitName], [CreatedTime], [IsDeleted]) VALUES (2, N'Vòng', SYSDATETIME(), 0)
GO
IF EXISTS (SELECT 1 FROM [dbo].[Units] WHERE [Id] = 3)
UPDATE [dbo].[Units] SET [UnitName] = N'Hộp', [IsDeleted] = 0 WHERE [Id] = 3
ELSE
INSERT [dbo].[Units] ([Id], [UnitName], [CreatedTime], [IsDeleted]) VALUES (3, N'Hộp', SYSDATETIME(), 0)
GO
IF EXISTS (SELECT 1 FROM [dbo].[Units] WHERE [Id] = 4)
UPDATE [dbo].[Units] SET [UnitName] = N'Bộ', [IsDeleted] = 0 WHERE [Id] = 4
ELSE
INSERT [dbo].[Units] ([Id], [UnitName], [CreatedTime], [IsDeleted]) VALUES (4, N'Bộ', SYSDATETIME(), 0)
GO

SET IDENTITY_INSERT [dbo].[Units] OFF
GO

SET IDENTITY_INSERT [dbo].[Products] ON
GO

IF EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 1)
UPDATE [dbo].[Products] SET [ProductName] = N'Vòng trầm hương tự nhiên', [Description] = N'Trầm hương thiên nhiên, mùi dịu nhẹ, thư giãn', [Ingredient] = N'100% trầm tự nhiên', [BurningTime] = N'2-3 giờ', [OriginalPrice] = CAST(500000.00 AS Decimal(18, 2)), [PriceAfterDiscount] = CAST(450000.00 AS Decimal(18, 2)), [ImageUrl] = N'/images/products/0dcbf2cf9b63a08adce1d360f20aaf84.jpg', [CategoryId] = 1, [UnitId] = 1, [IsDeleted] = 0, [ImageUrl2] = N'/images/products/1b357a51ba613daf5c692747d8112b40.jpg', [ImageUrl3] = N'/images/products/3e3e863a009e73db887c075a58d689f7.jpg', [ImageUrl4] = N'/images/products/493c6b7d1574c7ed5fd3f2ec8ab2f40d.jpg', [Origin] = N'Quảng Nam', [Quantity] = 50, [Rating] = 5, [ReviewCount] = 120, [SoldCount] = 300, [AgeYear] = N'5', [Length] = N'20cm', [OilContent] = N'Cao', [UsageTags] = N'Thiền, thư giãn', [Weight] = N'10g' WHERE [Id] = 1
ELSE
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (1, N'Vòng trầm hương tự nhiên', N'Trầm hương thiên nhiên, mùi dịu nhẹ, thư giãn', N'100% trầm tự nhiên', N'2-3 giờ', CAST(500000.00 AS Decimal(18, 2)), CAST(450000.00 AS Decimal(18, 2)), N'/images/products/0dcbf2cf9b63a08adce1d360f20aaf84.jpg', 1, 1, SYSDATETIME(), 0, N'/images/products/1b357a51ba613daf5c692747d8112b40.jpg', N'/images/products/3e3e863a009e73db887c075a58d689f7.jpg', N'/images/products/493c6b7d1574c7ed5fd3f2ec8ab2f40d.jpg', N'Quảng Nam', 50, 5, 120, 300, N'5', N'20cm', N'Cao', N'Thiền, thư giãn', N'10g')
GO
IF EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 2)
UPDATE [dbo].[Products] SET [ProductName] = N'Nhang trầm cao cấp', [Description] = N'Nhang sạch, không hóa chất', [Ingredient] = N'Trầm + keo bời lời', [BurningTime] = N'45 phút', [OriginalPrice] = CAST(200000.00 AS Decimal(18, 2)), [PriceAfterDiscount] = CAST(180000.00 AS Decimal(18, 2)), [ImageUrl] = N'/images/products/772e8043fbfb1523afdd18fa6cae722f.jpg', [CategoryId] = 1, [UnitId] = 1, [IsDeleted] = 0, [ImageUrl2] = N'/images/products/bb320a909e44e848181f4d17c2d4b021.jpg', [ImageUrl3] = N'/images/products/dd6c3e227d3eb09575aa403127f993cc.jpg', [ImageUrl4] = NULL, [Origin] = N'Khánh Hòa', [Quantity] = 100, [Rating] = 4, [ReviewCount] = 80, [SoldCount] = 200, [AgeYear] = N'3', [Length] = N'30cm', [OilContent] = N'Trung bình', [UsageTags] = N'Thờ cúng', [Weight] = N'200g' WHERE [Id] = 2
ELSE
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (2, N'Nhang trầm cao cấp', N'Nhang sạch, không hóa chất', N'Trầm + keo bời lời', N'45 phút', CAST(200000.00 AS Decimal(18, 2)), CAST(180000.00 AS Decimal(18, 2)), N'/images/products/772e8043fbfb1523afdd18fa6cae722f.jpg', 1, 1, SYSDATETIME(), 0, N'/images/products/bb320a909e44e848181f4d17c2d4b021.jpg', N'/images/products/dd6c3e227d3eb09575aa403127f993cc.jpg', NULL, N'Khánh Hòa', 100, 4, 80, 200, N'3', N'30cm', N'Trung bình', N'Thờ cúng', N'200g')
GO
IF EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 3)
UPDATE [dbo].[Products] SET [ProductName] = N'Trầm miếng nguyên khối', [Description] = N'Dùng xông hoặc trưng bày', [Ingredient] = N'Trầm tự nhiên', [BurningTime] = N'Không cố định', [OriginalPrice] = CAST(1500000.00 AS Decimal(18, 2)), [PriceAfterDiscount] = CAST(1400000.00 AS Decimal(18, 2)), [ImageUrl] = N'img3.jpg', [CategoryId] = 2, [UnitId] = 1, [IsDeleted] = 0, [ImageUrl2] = NULL, [ImageUrl3] = NULL, [ImageUrl4] = NULL, [Origin] = N'Lào', [Quantity] = 20, [Rating] = 5, [ReviewCount] = 40, [SoldCount] = 90, [AgeYear] = N'10', [Length] = N'10cm', [OilContent] = N'Rất cao', [UsageTags] = N'Xông, phong thủy', [Weight] = N'50g' WHERE [Id] = 3
ELSE
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (3, N'Trầm miếng nguyên khối', N'Dùng xông hoặc trưng bày', N'Trầm tự nhiên', N'Không cố định', CAST(1500000.00 AS Decimal(18, 2)), CAST(1400000.00 AS Decimal(18, 2)), N'img3.jpg', 2, 1, SYSDATETIME(), 0, NULL, NULL, NULL, N'Lào', 20, 5, 40, 90, N'10', N'10cm', N'Rất cao', N'Xông, phong thủy', N'50g')
GO
IF EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 4)
UPDATE [dbo].[Products] SET [ProductName] = N'Bột trầm hương', [Description] = N'Dùng đốt lò xông, mùi thơm mạnh', [Ingredient] = N'Trầm xay mịn', [BurningTime] = N'30 phút', [OriginalPrice] = CAST(300000.00 AS Decimal(18, 2)), [PriceAfterDiscount] = CAST(250000.00 AS Decimal(18, 2)), [ImageUrl] = N'img4.jpg', [CategoryId] = 2, [UnitId] = 1, [IsDeleted] = 0, [ImageUrl2] = NULL, [ImageUrl3] = NULL, [ImageUrl4] = NULL, [Origin] = N'Việt Nam', [Quantity] = 70, [Rating] = 4, [ReviewCount] = 60, [SoldCount] = 150, [AgeYear] = N'2', [Length] = N'--', [OilContent] = N'Trung bình', [UsageTags] = N'Xông nhà', [Weight] = N'100g' WHERE [Id] = 4
ELSE
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (4, N'Bột trầm hương', N'Dùng đốt lò xông, mùi thơm mạnh', N'Trầm xay mịn', N'30 phút', CAST(300000.00 AS Decimal(18, 2)), CAST(250000.00 AS Decimal(18, 2)), N'img4.jpg', 2, 1, SYSDATETIME(), 0, NULL, NULL, NULL, N'Việt Nam', 70, 4, 60, 150, N'2', N'--', N'Trung bình', N'Xông nhà', N'100g')
GO
IF EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 5)
UPDATE [dbo].[Products] SET [ProductName] = N'Dụng cụ đốt trầm', [Description] = N'Lò đốt bằng gốm cao cấp', [Ingredient] = N'Gốm sứ', [BurningTime] = N'--', [OriginalPrice] = CAST(400000.00 AS Decimal(18, 2)), [PriceAfterDiscount] = CAST(350000.00 AS Decimal(18, 2)), [ImageUrl] = N'img5.jpg', [CategoryId] = 3, [UnitId] = 1, [IsDeleted] = 0, [ImageUrl2] = NULL, [ImageUrl3] = NULL, [ImageUrl4] = NULL, [Origin] = N'Bát Tràng', [Quantity] = 30, [Rating] = 5, [ReviewCount] = 25, [SoldCount] = 70, [AgeYear] = NULL, [Length] = N'15cm', [OilContent] = NULL, [UsageTags] = N'Trang trí, xông trầm', [Weight] = N'500g' WHERE [Id] = 5
ELSE
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (5, N'Dụng cụ đốt trầm', N'Lò đốt bằng gốm cao cấp', N'Gốm sứ', N'--', CAST(400000.00 AS Decimal(18, 2)), CAST(350000.00 AS Decimal(18, 2)), N'img5.jpg', 3, 1, SYSDATETIME(), 0, NULL, NULL, NULL, N'Bát Tràng', 30, 5, 25, 70, NULL, N'15cm', NULL, N'Trang trí, xông trầm', N'500g')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 6)
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (6, N'Nhang trầm không tăm 30cm', N'Nhang trầm sạch, cháy đều, ít khói, phù hợp phòng thiền và phòng khách.', N'Bột trầm, keo bời lời', N'50 phút', CAST(260000.00 AS Decimal(18, 2)), CAST(230000.00 AS Decimal(18, 2)), N'/images/products/no-image.jpg', 2, 3, SYSDATETIME(), 0, NULL, NULL, NULL, N'Khánh Hòa', 45, 5, 34, 128, N'4', N'30cm', N'Trung bình', N'Thiền, thờ cúng, thư giãn', N'180g')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 7)
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (7, N'Nhang trầm có tăm phổ thông', N'Dòng nhang dễ dùng hằng ngày, hương dịu và giá tốt.', N'Bột trầm, tăm tre, keo bời lời', N'40 phút', CAST(180000.00 AS Decimal(18, 2)), CAST(150000.00 AS Decimal(18, 2)), N'/images/products/no-image.jpg', 2, 3, SYSDATETIME(), 0, NULL, NULL, NULL, N'Quảng Nam', 86, 4, 22, 96, N'2', N'28cm', N'Thấp', N'Thờ cúng, xông nhà', N'220g')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 8)
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (8, N'Nụ trầm tháp cao cấp', N'Nụ trầm dáng tháp, tỏa hương nhanh, phù hợp lư xông nhỏ.', N'Bột trầm tuyển, keo thực vật', N'25 phút', CAST(320000.00 AS Decimal(18, 2)), CAST(285000.00 AS Decimal(18, 2)), N'/images/products/no-image.jpg', 2, 3, SYSDATETIME(), 0, NULL, NULL, NULL, N'Bình Định', 64, 5, 41, 152, N'5', N'3cm', N'Cao', N'Thiền, thư giãn, xông phòng', N'100g')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 9)
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (9, N'Nụ trầm nón nhỏ', N'Nụ trầm cỡ nhỏ cho khay xông cá nhân, hương ấm và dễ chịu.', N'Bột trầm, keo bời lời', N'18 phút', CAST(210000.00 AS Decimal(18, 2)), CAST(185000.00 AS Decimal(18, 2)), N'/images/products/no-image.jpg', 2, 3, SYSDATETIME(), 0, NULL, NULL, NULL, N'Quảng Nam', 72, 4, 18, 88, N'3', N'2.4cm', N'Trung bình', N'Xông phòng, thư giãn', N'90g')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 10)
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (10, N'Bột trầm xông nhà 50g', N'Bột trầm mịn dùng cho lư điện hoặc than hoạt tính.', N'Trầm xay mịn', N'30 phút', CAST(220000.00 AS Decimal(18, 2)), CAST(195000.00 AS Decimal(18, 2)), N'/images/products/no-image.jpg', 1, 3, SYSDATETIME(), 0, NULL, NULL, NULL, N'Việt Nam', 38, 4, 27, 75, N'2', N'--', N'Trung bình', N'Xông nhà, khử mùi', N'50g')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 11)
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (11, N'Bột trầm xông nhà 100g', N'Gói bột trầm lớn cho gia đình hoặc spa nhỏ.', N'Trầm xay mịn', N'35 phút', CAST(380000.00 AS Decimal(18, 2)), CAST(340000.00 AS Decimal(18, 2)), N'/images/products/no-image.jpg', 1, 3, SYSDATETIME(), 0, NULL, NULL, NULL, N'Việt Nam', 28, 5, 19, 54, N'3', N'--', N'Trung bình', N'Xông nhà, spa', N'100g')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 12)
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (12, N'Trầm miếng vụn xông', N'Trầm miếng vụn tự nhiên, dễ chia liều khi xông.', N'Trầm miếng tự nhiên', N'Không cố định', CAST(620000.00 AS Decimal(18, 2)), CAST(560000.00 AS Decimal(18, 2)), N'/images/products/no-image.jpg', 1, 3, SYSDATETIME(), 0, NULL, NULL, NULL, N'Lào', 16, 5, 31, 63, N'8', N'1-4cm', N'Cao', N'Xông, phong thủy', N'30g')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 13)
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (13, N'Trầm miếng tuyển 20g', N'Miếng trầm tuyển hương sâu, dùng cho dịp lễ hoặc quà tặng.', N'Trầm tự nhiên tuyển chọn', N'Không cố định', CAST(980000.00 AS Decimal(18, 2)), CAST(890000.00 AS Decimal(18, 2)), N'/images/products/no-image.jpg', 1, 3, SYSDATETIME(), 0, NULL, NULL, NULL, N'Khánh Hòa', 12, 5, 24, 37, N'12', N'2-6cm', N'Rất cao', N'Quà tặng, phong thủy, xông', N'20g')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 14)
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (14, N'Vòng trầm 108 hạt 6mm', N'Vòng trầm 108 hạt nhỏ, hợp đeo hằng ngày và tụng niệm.', N'Trầm tự nhiên', N'--', CAST(780000.00 AS Decimal(18, 2)), CAST(690000.00 AS Decimal(18, 2)), N'/images/products/no-image.jpg', 1, 2, SYSDATETIME(), 0, NULL, NULL, NULL, N'Quảng Nam', 22, 5, 45, 81, N'6', N'6mm', N'Cao', N'Thiền, phong thủy, đeo tay', N'18g')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 15)
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (15, N'Vòng trầm 108 hạt 8mm', N'Vòng trầm cỡ 8mm, vân đẹp, mùi bền.', N'Trầm tự nhiên', N'--', CAST(1150000.00 AS Decimal(18, 2)), CAST(990000.00 AS Decimal(18, 2)), N'/images/products/no-image.jpg', 1, 2, SYSDATETIME(), 0, NULL, NULL, NULL, N'Gia Lai', 18, 5, 38, 66, N'8', N'8mm', N'Cao', N'Thiền, phong thủy, quà tặng', N'28g')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 16)
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (16, N'Vòng trầm đơn 10mm', N'Vòng đơn trầm 10mm dáng tối giản, hợp nam và nữ.', N'Trầm tự nhiên', N'--', CAST(860000.00 AS Decimal(18, 2)), CAST(760000.00 AS Decimal(18, 2)), N'/images/products/no-image.jpg', 1, 2, SYSDATETIME(), 0, NULL, NULL, NULL, N'Quảng Nam', 25, 4, 20, 49, N'5', N'10mm', N'Trung bình', N'Đeo tay, phong thủy', N'24g')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 17)
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (17, N'Mặt dây trầm hồ ly', N'Mặt dây trầm chạm hồ ly nhỏ gọn, có dây đeo kèm.', N'Trầm tự nhiên', N'--', CAST(520000.00 AS Decimal(18, 2)), CAST(470000.00 AS Decimal(18, 2)), N'/images/products/no-image.jpg', 1, 1, SYSDATETIME(), 0, NULL, NULL, NULL, N'Bình Định', 32, 4, 14, 35, N'4', N'3cm', N'Trung bình', N'Phong thủy, quà tặng', N'12g')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 18)
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (18, N'Mặt dây trầm Phật bản mệnh', N'Mặt dây chạm Phật bản mệnh, hương nhẹ, dễ phối dây.', N'Trầm tự nhiên', N'--', CAST(680000.00 AS Decimal(18, 2)), CAST(610000.00 AS Decimal(18, 2)), N'/images/products/no-image.jpg', 1, 1, SYSDATETIME(), 0, NULL, NULL, NULL, N'Gia Lai', 14, 5, 18, 28, N'7', N'3.5cm', N'Cao', N'Phong thủy, bình an', N'16g')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 19)
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (19, N'Tinh dầu trầm 5ml', N'Tinh dầu trầm nguyên chất dung tích nhỏ để trải nghiệm.', N'Tinh dầu trầm', N'--', CAST(390000.00 AS Decimal(18, 2)), CAST(350000.00 AS Decimal(18, 2)), N'/images/products/no-image.jpg', 1, 1, SYSDATETIME(), 0, NULL, NULL, NULL, N'Khánh Hòa', 20, 5, 29, 58, N'10', N'5ml', N'Rất cao', N'Xông tinh dầu, thư giãn', N'5ml')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 20)
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (20, N'Tinh dầu trầm 10ml', N'Tinh dầu trầm đậm hương, dùng với máy khuếch tán hoặc lư điện.', N'Tinh dầu trầm', N'--', CAST(720000.00 AS Decimal(18, 2)), CAST(650000.00 AS Decimal(18, 2)), N'/images/products/no-image.jpg', 1, 1, SYSDATETIME(), 0, NULL, NULL, NULL, N'Khánh Hòa', 4, 5, 36, 77, N'12', N'10ml', N'Rất cao', N'Xông tinh dầu, spa, thư giãn', N'10ml')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 21)
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (21, N'Lư xông trầm gốm trắng', N'Lư xông gốm men trắng, hợp nụ trầm và bột trầm.', N'Gốm sứ', N'--', CAST(260000.00 AS Decimal(18, 2)), CAST(225000.00 AS Decimal(18, 2)), N'/images/products/no-image.jpg', 3, 1, SYSDATETIME(), 0, NULL, NULL, NULL, N'Bát Tràng', 3, 4, 16, 42, NULL, N'12cm', NULL, N'Xông trầm, trang trí', N'420g')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 22)
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (22, N'Lư xông trầm gốm nâu', N'Lư xông gốm nâu trầm, kiểu dáng cổ điển.', N'Gốm sứ', N'--', CAST(280000.00 AS Decimal(18, 2)), CAST(245000.00 AS Decimal(18, 2)), N'/images/products/no-image.jpg', 3, 1, SYSDATETIME(), 0, NULL, NULL, NULL, N'Bát Tràng', 17, 4, 12, 31, NULL, N'13cm', NULL, N'Xông trầm, decor', N'460g')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 23)
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (23, N'Khay đốt trầm gỗ óc chó', N'Khay đốt trầm bằng gỗ, có rãnh giữ nhang.', N'Gỗ óc chó, hợp kim', N'--', CAST(340000.00 AS Decimal(18, 2)), CAST(299000.00 AS Decimal(18, 2)), N'/images/products/no-image.jpg', 3, 1, SYSDATETIME(), 0, NULL, NULL, NULL, N'Việt Nam', 26, 5, 11, 44, NULL, N'24cm', NULL, N'Đốt nhang, decor', N'300g')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 24)
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (24, N'Kẹp gắp than xông trầm', N'Kẹp inox nhỏ dùng gắp than hoạt tính và miếng trầm.', N'Inox', N'--', CAST(90000.00 AS Decimal(18, 2)), CAST(75000.00 AS Decimal(18, 2)), N'/images/products/no-image.jpg', 3, 1, SYSDATETIME(), 0, NULL, NULL, NULL, N'Việt Nam', 58, 4, 9, 39, NULL, N'16cm', NULL, N'Phụ kiện xông trầm', N'80g')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 25)
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (25, N'Than hoạt tính xông trầm', N'Than viên chuyên dùng cho trầm miếng và bột trầm.', N'Than hoạt tính', N'60 phút', CAST(120000.00 AS Decimal(18, 2)), CAST(99000.00 AS Decimal(18, 2)), N'/images/products/no-image.jpg', 3, 3, SYSDATETIME(), 0, NULL, NULL, NULL, N'Việt Nam', 95, 4, 13, 72, NULL, N'3.3cm', NULL, N'Xông trầm, phụ kiện', N'20 viên')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 26)
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (26, N'Hộp quà trầm an nhiên', N'Bộ quà gồm nụ trầm, lư gốm nhỏ và thiệp tặng.', N'Nụ trầm, gốm sứ, giấy mỹ thuật', N'25 phút', CAST(680000.00 AS Decimal(18, 2)), CAST(620000.00 AS Decimal(18, 2)), N'/images/products/no-image.jpg', 1, 4, SYSDATETIME(), 0, NULL, NULL, NULL, N'Việt Nam', 8, 5, 20, 33, N'5', N'Hộp 24x18cm', N'Cao', N'Quà tặng, thư giãn', N'900g')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 27)
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (27, N'Hộp quà trầm lễ Phật', N'Bộ quà trang nhã cho dịp lễ, gồm nhang trầm và phụ kiện đốt.', N'Nhang trầm, gỗ, giấy mỹ thuật', N'45 phút', CAST(820000.00 AS Decimal(18, 2)), CAST(760000.00 AS Decimal(18, 2)), N'/images/products/no-image.jpg', 1, 4, SYSDATETIME(), 0, NULL, NULL, NULL, N'Khánh Hòa', 2, 5, 17, 28, N'6', N'Hộp 28x20cm', N'Cao', N'Quà tặng, thờ cúng', N'1.1kg')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 28)
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (28, N'Combo thiền buổi tối', N'Combo nụ trầm, khay đốt và than hoạt tính cho thói quen thư giãn.', N'Nụ trầm, khay gỗ, than hoạt tính', N'25-60 phút', CAST(560000.00 AS Decimal(18, 2)), CAST(499000.00 AS Decimal(18, 2)), N'/images/products/no-image.jpg', 1, 4, SYSDATETIME(), 0, NULL, NULL, NULL, N'Việt Nam', 19, 5, 21, 46, N'4', N'Bộ tiêu chuẩn', N'Trung bình', N'Thiền, thư giãn, xông phòng', N'750g')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 29)
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (29, N'Combo xông nhà đầu tháng', N'Bộ bột trầm, than và kẹp gắp dùng cho nghi thức xông nhà.', N'Bột trầm, than hoạt tính, kẹp inox', N'30-60 phút', CAST(430000.00 AS Decimal(18, 2)), CAST(389000.00 AS Decimal(18, 2)), N'/images/products/no-image.jpg', 1, 4, SYSDATETIME(), 0, NULL, NULL, NULL, N'Việt Nam', 24, 4, 15, 51, N'3', N'Bộ tiêu chuẩn', N'Trung bình', N'Xông nhà, phong thủy', N'650g')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 30)
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (30, N'Bộ lư điện xông trầm mini', N'Lư điện mini điều chỉnh nhiệt, tiện dùng cho văn phòng.', N'Gốm, hợp kim, linh kiện điện', N'--', CAST(690000.00 AS Decimal(18, 2)), CAST(620000.00 AS Decimal(18, 2)), N'/images/products/no-image.jpg', 3, 4, SYSDATETIME(), 0, NULL, NULL, NULL, N'Việt Nam', 11, 4, 10, 25, NULL, N'10cm', NULL, N'Xông trầm, văn phòng', N'520g')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 31)
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (31, N'Bộ lư điện xông trầm cao cấp', N'Lư điện dung tích lớn, giữ nhiệt ổn định cho không gian rộng.', N'Gốm, hợp kim, linh kiện điện', N'--', CAST(1250000.00 AS Decimal(18, 2)), CAST(1120000.00 AS Decimal(18, 2)), N'/images/products/no-image.jpg', 3, 4, SYSDATETIME(), 0, NULL, NULL, NULL, N'Việt Nam', 7, 5, 8, 18, NULL, N'16cm', NULL, N'Xông trầm, spa, phòng khách', N'1.2kg')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 32)
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (32, N'Túi thơm trầm treo xe', N'Túi thơm trầm nhỏ treo xe hoặc tủ áo, hương nhẹ.', N'Bột trầm, vải linen', N'--', CAST(120000.00 AS Decimal(18, 2)), CAST(99000.00 AS Decimal(18, 2)), N'/images/products/no-image.jpg', 1, 1, SYSDATETIME(), 0, NULL, NULL, NULL, N'Quảng Nam', 40, 4, 13, 68, N'2', N'8x10cm', N'Thấp', N'Treo xe, tủ áo, khử mùi', N'35g')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 33)
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (33, N'Túi thơm trầm tủ áo', N'Túi thơm kích thước lớn hơn, phù hợp tủ quần áo và ngăn kéo.', N'Bột trầm, vải cotton', N'--', CAST(150000.00 AS Decimal(18, 2)), CAST(129000.00 AS Decimal(18, 2)), N'/images/products/no-image.jpg', 1, 1, SYSDATETIME(), 0, NULL, NULL, NULL, N'Bình Định', 33, 4, 12, 47, N'2', N'10x12cm', N'Thấp', N'Tủ áo, khử mùi', N'55g')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 34)
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (34, N'Trầm khoanh xông lư', N'Trầm khoanh cắt lát, dễ xếp trong lư xông.', N'Trầm khoanh tự nhiên', N'Không cố định', CAST(760000.00 AS Decimal(18, 2)), CAST(690000.00 AS Decimal(18, 2)), N'/images/products/no-image.jpg', 1, 3, SYSDATETIME(), 0, NULL, NULL, NULL, N'Lào', 15, 5, 19, 34, N'9', N'2-5cm', N'Cao', N'Xông, phong thủy', N'25g')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Products] WHERE [Id] = 35)
INSERT [dbo].[Products] ([Id], [ProductName], [Description], [Ingredient], [BurningTime], [OriginalPrice], [PriceAfterDiscount], [ImageUrl], [CategoryId], [UnitId], [CreatedTime], [IsDeleted], [ImageUrl2], [ImageUrl3], [ImageUrl4], [Origin], [Quantity], [Rating], [ReviewCount], [SoldCount], [AgeYear], [Length], [OilContent], [UsageTags], [Weight]) VALUES (35, N'Trầm cảnh để bàn mini', N'Trầm cảnh nhỏ đặt bàn làm việc, có đế gỗ.', N'Trầm tự nhiên, đế gỗ', N'--', CAST(1450000.00 AS Decimal(18, 2)), CAST(1320000.00 AS Decimal(18, 2)), N'/images/products/no-image.jpg', 1, 1, SYSDATETIME(), 0, NULL, NULL, NULL, N'Quảng Nam', 6, 5, 7, 12, N'15', N'12cm', N'Rất cao', N'Trang trí, phong thủy, quà tặng', N'180g')
GO

SET IDENTITY_INSERT [dbo].[Products] OFF
GO
