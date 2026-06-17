# 🪔 Trầm Hương Shop — Phase 1: HTML/CSS Prototype

Website thương mại điện tử bán hương trầm thiên nhiên Việt Nam.

## 📁 Cấu trúc

```
01-prototype/
├── index.html              ← Trang chủ
├── san-pham.html           ← Danh sách sản phẩm + bộ lọc
├── chi-tiet.html           ← Chi tiết sản phẩm
├── gio-hang.html           ← Giỏ hàng
├── thanh-toan.html         ← Checkout (3 bước, SĐT)
├── dat-hang-thanh-cong.html← Đặt hàng thành công + gợi ý tạo TK
├── tra-cuu.html            ← Tra cứu đơn hàng bằng SĐT + OTP
└── dang-nhap.html          ← Đăng nhập / Đăng ký bằng SĐT
```

## 🚀 Cách chạy

Mở thẳng file HTML trong trình duyệt — không cần server, không cần cài đặt.

```bash
# Hoặc dùng Live Server (VS Code extension)
open index.html
```

## ✨ Tính năng nổi bật

- **Guest Checkout bằng SĐT** — mua hàng không cần tài khoản
- **Lịch sử đơn theo SĐT** — tra cứu đơn bằng OTP xác thực
- **Gợi ý tạo tài khoản** — sau khi đặt hàng thành công
- **Giỏ hàng** lưu trong localStorage (30 ngày)
- **Flash Sale** với đồng hồ đếm ngược
- **Bộ lọc sản phẩm** — danh mục, xuất xứ, giá, mục đích, đánh giá
- **Responsive** — hoạt động tốt trên mobile
- **OTP flow** — đăng nhập / đăng ký / tra cứu đơn

## 🛣️ Lộ trình

| Phase | Mô tả | Trạng thái |
|-------|-------|-----------|
| **Phase 1** | HTML/CSS Prototype (file này) | ✅ Hoàn thành |
| **Phase 2** | ASP.NET Core + EF Core + Database | 🔄 Tiếp theo |
| **Phase 3** | Razor Pages kết nối backend | ⏳ Chờ |
| **Phase 4** | Admin Dashboard + Deploy CI/CD | ⏳ Chờ |

## 🎨 Thiết kế

- **Font**: Cormorant Garamond (serif) + Be Vietnam Pro (sans)
- **Màu chủ đạo**: Nâu trầm `#1C1209` + Vàng gold `#B8860B`
- **Phong cách**: Ấm áp, thiên nhiên, tinh tế — phù hợp sản phẩm hương trầm

## 📦 Tech Stack (Phase 1)

- HTML5 + CSS3 thuần (CSS Variables, Grid, Flexbox)
- Vanilla JavaScript (không framework)
- Google Fonts
- localStorage cho giỏ hàng

---
*Dự án cá nhân — Trầm Hương Shop © 2025*
