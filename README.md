# Agarwood & Incense E-Commerce Platform

## 1. Giới thiệu

Hệ thống bán hàng trầm hương / nhang, kiến trúc nhiều lớp, **tách cửa hàng khách và backoffice**.

## 2. Kiến trúc solution

| Project | Vai trò |
|---------|---------|
| `SV22T1080045.Shop.Web` | Cửa hàng: sản phẩm, giỏ, checkout, thanh toán |
| `SV22T1080045.Shop.Admin` | Backoffice: Management, Staff, báo cáo |
| `SV22T1080045.Shop.Payments` | VNPay, MoMo, VietQR (QR chuyển khoản) |
| `SV22T1080045.Shop.BusinessLayers` | Nghiệp vụ |
| `SV22T1080045.Shop.DataLayers` | EF Core / DAL |
| `SV22T1080045.Shop.Models` | ViewModel, request |
| `SV22T1080045.Shop.DomainModels` | Entity |
| `SV22T1080045.Shop.Abstractions` | Contract, DTO |

SDK: .NET 8 (`global.json`).

**URL dev mặc định**

- Cửa hàng: https://localhost:7126 (`SV22T1080045.Shop.Web`)
- Quản trị: https://localhost:7127 (`SV22T1080045.Shop.Admin`)

Chi tiết kiểm thử và truy cập từ điện thoại: **[TESTING.md](TESTING.md)**.

## 3. Chạy dự án

```powershell
dotnet ef database update --project SV22T1080045.Shop.DataLayers --startup-project SV22T1080045.Shop.Web

dotnet run --project SV22T1080045.Shop.Web
dotnet run --project SV22T1080045.Shop.Admin
```

Cấu hình DB: `appsettings.json` trong **Web** và **Admin** (cùng connection string).

## 4. Thanh toán — cấu hình trong `SV22T1080045.Shop.Web/appsettings.json`

### VNPay (bạn tự đăng ký merchant)

1. [VNPay Sandbox](https://sandbox.vnpayment.vn/devreg/)
2. Điền `VnPay:TmnCode`, `VnPay:HashSecret`
3. Return URL: `https://<host>/Checkout/VnPayReturn`

### MoMo (bạn tự đăng ký)

1. [MoMo Developer / Business](https://developers.momo.vn/) — tạo app sandbox
2. Điền `MoMo:PartnerCode`, `MoMo:AccessKey`, `MoMo:SecretKey`
3. Trên cổng MoMo khai báo:
   - Redirect: `https://<host>/Checkout/MoMoReturn`
   - IPN: `https://<host>/Checkout/MoMoIpn`

### Chuyển khoản QR (VietQR — bạn tự điền tài khoản nhận)

Không cần mua dịch vụ riêng; cần **tài khoản ngân hàng** và mã **BIN** ngân hàng (tra [VietQR](https://www.vietqr.io/)).

```json
"VietQr": {
  "BankId": "970436",
  "AccountNumber": "0123456789",
  "AccountName": "TRAN HUONG SHOP",
  "Template": "compact2"
}
```

Khách chọn “Chuyển khoản QR” → màn hình QR có **sẵn số tiền** và nội dung `TH000123`. Shop xác nhận thanh toán thủ công trên Staff (chưa có webhook ngân hàng).

**Lưu ý:** Ảnh QR lấy từ `img.vietqr.io` (cần internet). Production có thể thay bằng API VietQR chính thức nếu bạn đăng ký.

## 5. Giá sản phẩm (quản trị)

| Trường | Ý nghĩa |
|--------|---------|
| `ImportPrice` | Giá nhập — nội bộ |
| `SalePrice` | Giá niêm yết |
| `DiscountPercent` | % giảm |
| `PriceAfterDiscount` | Giá khách trả |
| `ProductInventories` | Tồn kho (bảng riêng) |

Migration / script: `SV22T1080045.Shop.DataLayers/Scripts/MigrateProductPricingAndInventory.sql`
