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
3. (Tuỳ chọn) Seed dữ liệu mẫu: `SV22T1080045.Shop.DataLayers/Scripts/SeedProducts_30.sql`
4. Ảnh sản phẩm upload vào `wwwroot/images/products/` (thư mục không commit lên Git).
