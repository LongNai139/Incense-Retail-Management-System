# Hướng dẫn kiểm thử (local & thiết bị khác)

## 1. Hai ứng dụng tách riêng

| Ứng dụng | Project | URL mặc định (HTTPS) | Vai trò |
|----------|---------|----------------------|---------|
| **Cửa hàng (khách)** | `SV22T1080045.Shop.Web` | https://localhost:7126 | Mua hàng, giỏ, thanh toán, tra cứu đơn |
| **Backoffice** | `SV22T1080045.Shop.Admin` | https://localhost:7127 | Quản lý, Staff, báo cáo, cảnh báo |

Chạy **cả hai** khi kiểm thử đầy đủ:

```powershell
# Terminal 1 – cửa hàng
dotnet run --project SV22T1080045.Shop.Web

# Terminal 2 – quản trị
dotnet run --project SV22T1080045.Shop.Admin
```

Hoặc trong Visual Studio: đặt **Multiple startup projects** → Web + Admin.

## 2. Truy cập từ điện thoại / máy khác (cùng Wi‑Fi)

**Có**, bạn cần cho phép máy khác truy cập qua IP LAN, không chỉ `localhost`.

### Bước A – Lấy IP máy dev

```powershell
ipconfig
```

Ghi lại IPv4 (ví dụ `192.168.1.50`).

### Bước B – Bind 0.0.0.0

Sửa `Properties/launchSettings.json` của **Web** và **Admin**:

```json
"applicationUrl": "https://0.0.0.0:7126;http://0.0.0.0:5299"
```

(tương tự Admin: `7127` / `5298`)

Hoặc chạy một lần:

```powershell
dotnet run --project SV22T1080045.Shop.Web --urls "https://0.0.0.0:7126;http://0.0.0.0:5299"
```

### Bước C – Firewall Windows

Cho phép inbound port **7126**, **7127** (và HTTP nếu dùng).

### Bước D – HTTPS trên điện thoại

Certificate dev của .NET thường **không tin cậy** trên điện thoại. Có thể:

- Dùng **HTTP** khi test nội bộ: `http://192.168.1.50:5299` (chỉ mạng tin cậy), hoặc
- Cài/cấp cert dev cho IP (phức tạp hơn), hoặc
- Dùng **ngrok** / tunnel (xem mục 5).

### Bước E – Cập nhật URL callback thanh toán

Trong `SV22T1080045.Shop.Web/appsettings.json`:

```json
"AppHosts": {
  "StorefrontUrl": "https://192.168.1.50:7126",
  "BackofficeUrl": "https://192.168.1.50:7127"
}
```

**VNPay / MoMo**: đăng ký Return URL / IPN URL trỏ tới IP/domain thật, ví dụ:

- `https://192.168.1.50:7126/Checkout/VnPayReturn`
- `https://192.168.1.50:7126/Checkout/MoMoReturn`
- `https://192.168.1.50:7126/Checkout/MoMoIpn`

Sandbox có thể **không chấp nhận IP LAN** — khi đó dùng **ngrok** (mục 5).

## 3. Checklist kiểm thử chức năng

### Cửa hàng (Web – :7126)

- [ ] Trang chủ, danh sách sản phẩm, chi tiết sản phẩm
- [ ] Thêm giỏ, sửa số lượng, voucher trên giỏ
- [ ] Checkout COD → trang thành công
- [ ] Checkout **VNPay** (sau khi cấu hình TmnCode) → redirect sandbox → quay lại Success
- [ ] Checkout **MoMo** (sau khi cấu hình PartnerCode) → redirect MoMo test → quay lại
- [ ] Checkout **Chuyển khoản QR** (sau khi cấu hình VietQR) → màn hình QR đúng số tiền + nội dung `TH000xxx`
- [ ] Tra cứu đơn bằng SĐT
- [ ] Đăng ký / đăng nhập khách

### Backoffice (Admin – :7127)

- [ ] `/` hoặc `/Management` — dashboard
- [ ] Staff — đơn hàng, âm báo (bật nút Âm báo, tạo đơn mới từ Web)
- [ ] Cập nhật trạng thái đơn, tồn kho
- [ ] Đăng nhập tài khoản Admin/Staff (cookie **khác** cửa hàng)

## 4. Cấu hình thanh toán trước khi test

Xem `README.md`:

- **VNPay**: TmnCode, HashSecret
- **MoMo**: PartnerCode, AccessKey, SecretKey + IPN URL trên cổng MoMo
- **VietQR**: BankId (BIN), AccountNumber, AccountName — bạn tự điền tài khoản nhận tiền thật

QR dùng dịch vụ ảnh VietQR (`img.vietqr.io`); cần **internet** khi hiển thị mã.

## 5. Tunnel công khai (khuyến nghị cho VNPay/MoMo)

Khi cổng thanh toán yêu cầu URL public:

```powershell
ngrok http 7126
```

Lấy URL `https://xxxx.ngrok-free.app` → cập nhật `AppHosts:StorefrontUrl` và Return/IPN trên cổng VNPay/MoMo.

## 6. Database

Đảm bảo SQL Server chạy và connection string đúng trên **cả hai** `appsettings.json` (Web + Admin).

```powershell
dotnet ef database update --project SV22T1080045.Shop.DataLayers --startup-project SV22T1080045.Shop.Web
```

## 7. Lưu ý cookie đăng nhập

- Khách: cookie `SV22T1080045_Shop_Customer_Auth` (Web)
- Staff/Admin: cookie `SV22T1080045_Shop_Backoffice_Auth` (Admin)

Đăng nhập trên Web **không** tự đăng nhập Admin (khác port và tên cookie) — đúng thiết kế.
