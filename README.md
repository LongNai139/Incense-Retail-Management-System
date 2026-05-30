# Agarwood & Incense E-Commerce Platform

## 1. Giới thiệu
Đây là hệ thống bán hàng dành cho cửa hàng trầm hương và nhang, được tổ chức theo kiến trúc nhiều lớp để tách biệt phần giao diện, nghiệp vụ và truy cập dữ liệu.

## 2. Kiến trúc
Solution hiện gồm các project chính:

- `SV22T1080045.Shop.Abstractions`: hợp đồng (interface), DTO/query dùng chung giữa các lớp.
- `SV22T1080045.Shop.DomainModels`: các thực thể cốt lõi.
- `SV22T1080045.Shop.DataLayers`: truy cập dữ liệu bằng EF Core và Dapper.
- `SV22T1080045.Shop.BusinessLayers`: xử lý nghiệp vụ.
- `SV22T1080045.Shop.Models`: view model cho phần giao diện.
- `SV22T1080045.Shop.Admin`: ứng dụng ASP.NET Core MVC.

Yêu cầu SDK: .NET 8 (xem `global.json`).

## 3. Chức năng chính

- Thanh toán không cần tài khoản (guest checkout).
- Tra cứu đơn hàng bằng số điện thoại.
- Thanh toán VNPay (sandbox).
- Quản lý sản phẩm, danh mục, nhân viên và báo cáo.
- Đăng ký tài khoản có xác thực OTP.

## 4. Thiết lập nhanh

1. Cấu hình chuỗi kết nối trong `SV22T1080045.Shop.Admin/appsettings.json`.
2. Chạy migration: `dotnet ef database update --project SV22T1080045.Shop.DataLayers --startup-project SV22T1080045.Shop.Admin`
3. Backfill giá/tồn kho (sau migration mới): `SV22T1080045.Shop.DataLayers/Scripts/MigrateProductPricingAndInventory.sql`
4. (Tuỳ chọn) Seed dữ liệu mẫu: `SV22T1080045.Shop.DataLayers/Scripts/SeedProducts_30.sql`
5. Ảnh sản phẩm upload vào `wwwroot/images/products/` (thư mục không commit lên Git).

### Thanh toán VNPay (bạn cần tự đăng ký)

Ứng dụng đã tích hợp luồng redirect VNPay sandbox. Để bật thanh toán online:

1. Đăng ký merchant tại [VNPay Sandbox](https://sandbox.vnpayment.vn/devreg/) (hoặc tài khoản production khi go-live).
2. Lấy **TmnCode** và **HashSecret** từ cổng VNPay.
3. Cập nhật `SV22T1080045.Shop.Admin/appsettings.json` (hoặc User Secrets / biến môi trường trên server):

```json
"VnPay": {
  "TmnCode": "MÃ_CỦA_BẠN",
  "HashSecret": "SECRET_CỦA_BẠN",
  "PaymentUrl": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html"
}
```

4. Cấu hình **Return URL** trên cổng VNPay trùng với: `https://<host-của-bạn>/Checkout/VnPayReturn` (local: `https://localhost:<port>/Checkout/VnPayReturn`).
5. Khi deploy production: đổi `PaymentUrl` sang URL production VNPay và dùng merchant production.

Nếu chưa cấu hình `TmnCode`/`HashSecret`, khách vẫn đặt hàng bằng COD; chọn VNPay sẽ báo lỗi cấu hình.

### Giá sản phẩm (quản trị)

| Trường | Ý nghĩa |
|--------|---------|
| `ImportPrice` | Giá nhập — chỉ dùng nội bộ (Management), không hiển thị khách |
| `SalePrice` | Giá bán niêm yết trước giảm |
| `DiscountPercent` | % giảm do admin thiết lập |
| `PriceAfterDiscount` | Giá khách trả (tự tính khi lưu; giữ tương thích dữ liệu cũ) |
| `OriginalPrice` | Giữ cho dữ liệu legacy (đồng bộ với `SalePrice` khi lưu) |
| `ProductInventories` | Tồn kho tách bảng; `Products.Quantity` vẫn giữ để không vỡ dữ liệu cũ |
