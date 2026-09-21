# Hướng dẫn kiểm thử đầy đủ (máy tính + điện thoại cùng Wi‑Fi)

Tài liệu này giả định bạn đã clone solution và có SQL Server chạy được. Nếu bạn **chỉ mới sửa `launchSettings.json` (bước B)** mà điện thoại vẫn không vào được — làm tiếp **bước C → H** theo đúng thứ tự bên dưới.

---

## Mục lục

1. [Hai app cần chạy](#1-hai-app-cần-chạy)
2. [Chuẩn bị trước khi test](#2-chuẩn-bị-trước-khi-test)
3. [Test trên máy tính (localhost)](#3-test-trên-máy-tính-localhost) — gồm [3.4 Checklist phân quyền](#34-checklist-phân-quyền-trên-một-máy-localhost)
4. [Test từ điện thoại — từng bước](#4-test-từ-điện-thoại--từng-bước)
   - A. Lấy IP máy dev  
   - B. `launchSettings` (bạn đã làm)  
   - C. Mở Firewall Windows  
   - D. Chạy app và kiểm tra đang lắng nghe  
   - E. Test bằng HTTP trên điện thoại (khuyến nghị)  
   - F. Cập nhật `appsettings`  
   - G. Test HTTPS (tùy chọn, khó hơn trên điện thoại)  
   - H. Xác nhận thành công  
5. [Visual Studio — chạy đúng cách](#5-visual-studio--chạy-đúng-cách)
6. [PowerShell — chạy thủ công](#6-powershell--chạy-thủ-công)
7. [Checklist chức năng](#7-checklist-chức-năng)
8. [Thanh toán VNPay / MoMo / QR](#8-thanh-toán-vnpay--momo--qr)
9. [Ngrok (khi cần URL công khai)](#9-ngrok-khi-cần-url-công-khai)
10. [Xử lý lỗi thường gặp](#10-xử-lý-lỗi-thường-gặp)

---

## 1. Hai app cần chạy

| Ứng dụng | Project | Cổng HTTPS | Cổng HTTP | Vai trò |
|----------|---------|------------|-----------|---------|
| **Cửa hàng** | `SV22T1080045.Shop.Web` | 7126 | **5299** | Khách mua hàng, thanh toán |
| **Quản trị** | `SV22T1080045.Shop.Admin` | 7127 | **5298** | Management, Staff |

- Trên **máy tính**: dùng `https://localhost:7126` hoặc `http://localhost:5299`.
- Trên **điện thoại**: nên dùng **HTTP + IP LAN** (ví dụ `http://192.168.1.50:5299`) — tránh lỗi chứng chỉ HTTPS.

`localhost` trên điện thoại **không** trỏ về máy tính của bạn — phải dùng **IPv4 Wi‑Fi** (dạng `192.168.x.x`).

---

## 2. Chuẩn bị trước khi test

### 2.1. Phần mềm

- [ ] .NET 8 SDK (`dotnet --version`)
- [ ] SQL Server đang chạy (LocalDB / SQL Express / full SQL)
- [ ] Visual Studio 2022 **hoặc** VS Code + terminal

### 2.2. Database

Mở PowerShell tại thư mục solution:

```powershell
cd C:\Users\ADMIN\source\repos\LongNai139\Incense-Retail-Management-System

dotnet ef database update `
  --project SV22T1080045.Shop.DataLayers `
  --startup-project SV22T1080045.Shop.Web
```

Nếu lỗi kết nối: sửa `ConnectionStrings:ShopConnectionString` trong:

- `SV22T1080045.Shop.Web\appsettings.json`
- `SV22T1080045.Shop.Admin\appsettings.json`

(hai file phải **cùng** chuỗi kết nối).

### 2.3. Mạng

- [ ] Máy tính và điện thoại **cùng Wi‑Fi** (không dùng 4G cho điện thoại khi test LAN).
- [ ] Tránh Wi‑Fi khách (guest) — nhiều router **cô lập** thiết bị khách, điện thoại không ping được PC.
- [ ] Tắt VPN trên máy tính (hoặc cho phép LAN) khi test.

---

## 3. Test trên máy tính (localhost)

Làm xong bước này rồi mới test điện thoại.

### 3.1. Chạy Web

**Visual Studio:** chọn startup project `SV22T1080045.Shop.Web` → profile **https** → F5.

**Hoặc PowerShell:**

```powershell
dotnet run --project SV22T1080045.Shop.Web --launch-profile https
```

Mở trình duyệt trên PC:

- https://localhost:7126 — trang chủ cửa hàng  
- https://localhost:7127 — chỉ khi đã chạy thêm Admin

### 3.2. Chạy Admin (khi test Staff / Quản lý)

```powershell
dotnet run --project SV22T1080045.Shop.Admin --launch-profile https
```

- https://localhost:7127 → Management / Staff

### 3.3. PC vẫn lỗi?

Sửa DB / build trước:

```powershell
dotnet build SV22T1080045.Shop.sln
```

Nếu báo **file locked** / MSB3026: **Stop debugging** (Shift+F5) trong Visual Studio rồi build lại.

### 3.4. Checklist phân quyền trên một máy (localhost)

Chạy **cả Web + Admin** (mục [5.2](#52-hai-project-web--admin)). Dùng **Chrome** cho cửa hàng và **Edge** (hoặc cửa sổ ẩn danh) cho quản trị — tránh lẫn cookie cùng trình duyệt.

| # | Việc | URL | Tài khoản | Kỳ vọng |
|---|------|-----|-----------|---------|
| 1 | Khách mua hàng | `https://localhost:7126` | Đăng ký mới hoặc khách | Vào được; **không** có menu Staff/Quản lý |
| 2 | Khách vào backoffice | `https://localhost:7127/Account/Login` | SĐT khách | Báo chỉ đăng nhập tại cửa hàng + link `7126` |
| 3 | Staff đăng nhập | `https://localhost:7127/Account/Login` | `0911000001` / `123` | Vào `/Staff`; **không** vào `/Management` |
| 4 | Staff thử Management | `https://localhost:7127/Management` | (đã login Staff) | Trang **AccessDenied** + nút về Staff |
| 5 | Admin đăng nhập | `https://localhost:7127/Account/Login` | `0909123456` / `123` | Vào `/Management` và `/Staff` |
| 6 | Admin từ cửa hàng | `https://localhost:7126/Account/Login` | `0909123456` / `123` | Trang bridge → tự sang `7127` (vài giây) |
| 7 | Đăng xuất cửa hàng (Staff/Admin đã bridge) | `7126` → Đăng xuất | — | Gọi bridge xóa cookie `7127` |
| 8 | Đăng xuất quản trị | `7127` → Đăng xuất | — | Gọi bridge xóa cookie `7126`, về login cửa hàng |

**Cookie (tên kỹ thuật):**

| Cổng | Cookie auth | Cookie session |
|------|-------------|----------------|
| 7126 Web | `SV22T1080045_Shop_Customer_Auth` | `SV22T1080045_Shop_Web_Session` |
| 7127 Admin | `SV22T1080045_Shop_Backoffice_Auth` | `SV22T1080045_Shop_Admin_Session` |

**Tài khoản mẫu**

| Role | SĐT | Mật khẩu | Nguồn |
|------|-----|----------|--------|
| Admin | `0909123456` | `123` | Seed EF (`ShopDbContext`) — cần đã `database update` |
| Staff | `0911000001`, `0911000002` | `123` | `StartupSeedService` khi **Admin** app khởi động |

**Cấu hình host** (`appsettings.json` — Web **và** Admin phải giống nhau khi đổi IP/LAN):

```json
"AppHosts": {
  "StorefrontUrl": "https://localhost:7126",
  "BackofficeUrl": "https://localhost:7127"
}
```

**`launchSettings` (profile https):**

| Project | HTTPS | HTTP | Ghi chú |
|---------|-------|------|---------|
| Shop.Web | `https://0.0.0.0:7126` | `http://0.0.0.0:5299` | Mở trình duyệt → `/` |
| Shop.Admin | `https://0.0.0.0:7127` | `http://0.0.0.0:5298` | `launchUrl`: `Account/Login` |

---

## 4. Test từ điện thoại — từng bước

### Bước A — Lấy IPv4 máy dev

1. Nhấn `Win + R` → gõ `cmd` → Enter.  
2. Gõ:

```text
ipconfig
```

3. Tìm card **Wi‑Fi** (hoặc **Ethernet** nếu dây LAN):

```text
Wireless LAN adapter Wi-Fi:
   IPv4 Address. . . . . . . . . . . : 192.168.1.50
```

4. **Ghi lại số IPv4** — ví dụ dùng trong doc: `192.168.1.50`.  
   Thay **mọi chỗ** `192.168.1.50` bằng IP **của bạn**.

**Kiểm tra nhanh trên PC:** mở trình duyệt:

```text
http://192.168.1.50:5299
```

(Nếu app chưa chạy sẽ lỗi — bình thường; sau bước D sẽ vào được.)

---

### Bước B — `launchSettings.json` (bạn đã làm phần HTTPS)

Trong repo, profile **https** đã bind `0.0.0.0`:

**Web** — `SV22T1080045.Shop.Web\Properties\launchSettings.json`:

```json
"applicationUrl": "https://0.0.0.0:7126;http://0.0.0.0:5299"
```

**Admin** — `SV22T1080045.Shop.Admin\Properties\launchSettings.json`:

```json
"applicationUrl": "https://0.0.0.0:7127;http://0.0.0.0:5298"
```

`0.0.0.0` nghĩa là “lắng nghe mọi card mạng”, không chỉ localhost — **bắt buộc** để thiết bị khác trong LAN truy cập được.

**Lưu ý:** Profile **http** trong file (nếu còn `localhost`) chỉ dùng khi chọn profile `http` trong VS. Để test điện thoại, dùng profile **https** (đã có cả HTTP 5299) hoặc lệnh PowerShell ở mục 6.

**Sau khi sửa `launchSettings`:** tắt app (Stop debugging) → chạy lại (F5).

---

### Bước C — Mở Firewall Windows (bước hay bị bỏ qua)

Windows thường **chặn** máy ngoài vào port 5299/5298/7126/7127.

#### Cách 1: PowerShell (Administrator) — khuyến nghị

1. Start → gõ **PowerShell** → chuột phải → **Run as administrator**.  
2. Chạy lần lượt (copy cả khối):

```powershell
New-NetFirewallRule -DisplayName "TramHuong Shop Web HTTP"  -Direction Inbound -Protocol TCP -LocalPort 5299 -Action Allow
New-NetFirewallRule -DisplayName "TramHuong Shop Web HTTPS" -Direction Inbound -Protocol TCP -LocalPort 7126 -Action Allow
New-NetFirewallRule -DisplayName "TramHuong Shop Admin HTTP"  -Direction Inbound -Protocol TCP -LocalPort 5298 -Action Allow
New-NetFirewallRule -DisplayName "TramHuong Shop Admin HTTPS" -Direction Inbound -Protocol TCP -LocalPort 7127 -Action Allow
```

Nếu báo rule đã tồn tại → bỏ qua.

#### Cách 2: Giao diện Windows

1. **Windows Security** → **Firewall & network protection** → **Advanced settings**.  
2. **Inbound Rules** → **New Rule…** → Port → TCP → Specific ports: `5299` → Allow → đặt tên `Shop Web HTTP`.  
3. Lặp lại cho `5298`, `7126`, `7127` nếu cần HTTPS.

#### Cách 3: Khi chạy app lần đầu

Đôi khi Windows hiện popup **“Windows Defender Firewall has blocked…”** → tick **Private networks** → **Allow access**. Nếu đã từ chối trước đó, dùng Cách 1.

---

### Bước D — Chạy app và xác nhận đang lắng nghe

1. **Tắt hết** instance Web/Admin đang chạy (nhiều cửa sổ VS/terminal).  
2. Chạy lại Web (profile **https**).  
3. Trong cửa sổ terminal / Output, tìm dòng kiểu:

```text
Now listening on: https://0.0.0.0:7126
Now listening on: http://0.0.0.0:5299
```

Nếu chỉ thấy `https://localhost:7126` → profile chưa đúng hoặc chưa restart sau khi sửa `launchSettings`.

4. **Trên cùng máy PC**, mở trình duyệt:

| URL | Kỳ vọng |
|-----|---------|
| `http://192.168.1.50:5299` | Trang cửa hàng (HTTP) |
| `https://192.168.1.50:7126` | Trang cửa hàng (HTTPS, có thể cảnh báo cert) |

Nếu **PC dùng IP cũng không vào được** → chưa listen `0.0.0.0` hoặc firewall (quay lại B, C).  
Nếu **PC vào được IP nhưng điện thoại không** → firewall, khác Wi‑Fi, hoặc guest network (mục 10).

5. (Tùy chọn) Admin:

```powershell
dotnet run --project SV22T1080045.Shop.Admin --launch-profile https
```

Test PC: `http://192.168.1.50:5298` hoặc `https://192.168.1.50:7127`.

---

### Bước E — Mở trên điện thoại bằng HTTP (khuyến nghị)

1. Điện thoại **tắt 4G**, bật **cùng Wi‑Fi** với máy dev.  
2. Mở **Chrome / Safari**, gõ **chính xác** (thay IP của bạn):

```text
http://192.168.1.50:5299
```

3. Không thêm `https`, không thêm `/` thừa ở đầu.  
4. Trang chủ Trầm Hương Shop hiện ra → **thành công**.

**Admin trên điện thoại (nếu cần):**

```text
http://192.168.1.50:5298
```

**Liên kết Staff/Quản lý trên cửa hàng:** sau bước F, menu trên Web sẽ trỏ đúng IP Admin.

---

### Bước F — Cập nhật `appsettings.json`

Sửa **cả hai** file (Web bắt buộc; Admin nếu dùng link chéo):

`SV22T1080045.Shop.Web\appsettings.json`:

```json
"AppHosts": {
  "StorefrontUrl": "http://192.168.1.50:5299",
  "BackofficeUrl": "http://192.168.1.50:5298"
}
```

`SV22T1080045.Shop.Admin\appsettings.json` — thêm hoặc sửa tương tự:

```json
"AppHosts": {
  "StorefrontUrl": "http://192.168.1.50:5299",
  "BackofficeUrl": "http://192.168.1.50:5298"
}
```

Dùng `http://` + cổng **5299/5298** khi test LAN bằng HTTP.  
**Restart** cả Web và Admin sau khi sửa.

---

### Bước G — HTTPS trên điện thoại (tùy chọn, thường khó)

URL: `https://192.168.1.50:7126`

Trình duyệt điện thoại **không tin** chứng chỉ dev của .NET → báo “Kết nối không riêng tư” / không mở được.

**Trên PC (một lần):**

```powershell
dotnet dev-certs https --trust
```

Điện thoại **vẫn** có thể không chấp nhận — đó là bình thường.

**Cách xử lý nếu bắt buộc HTTPS trên điện thoại:**

- Dùng **ngrok** (mục 9), hoặc  
- Chỉ test HTTPS trên PC; điện thoại dùng HTTP port 5299.

---

### Bước H — Xác nhận đã test LAN thành công

| Việc | URL điện thoại | OK? |
|------|----------------|-----|
| Trang chủ | `http://<IP>:5299` | ☐ |
| Sản phẩm / giỏ | cùng site | ☐ |
| Admin (nếu cần) | `http://<IP>:5298` | ☐ |
| Đặt đơn COD | checkout trên điện thoại | ☐ |

---

## 5. Visual Studio — chạy đúng cách

### 5.1. Một project (chỉ cửa hàng)

1. Solution Explorer → chuột phải **SV22T1080045.Shop.Web** → **Set as Startup Project**.  
2. Thanh toolbar: dropdown profile chọn **https** (không chọn `http` nếu profile http vẫn là localhost).  
3. **F5**.

### 5.2. Hai project (Web + Admin)

1. Chuột phải **Solution** → **Properties**.  
2. **Startup Project** → **Multiple startup projects**.  
3. **SV22T1080045.Shop.Web** → **Start**  
4. **SV22T1080045.Shop.Admin** → **Start**  
5. **OK** → F5.

### 5.3. Sau khi đổi `launchSettings`

- **Stop** (Shift+F5) rồi **F5** lại — IIS Express/Kestrel không tự reload URL listen.

---

## 6. PowerShell — chạy thủ công

Mở **hai** cửa sổ PowerShell tại thư mục solution:

**Cửa sổ 1 — Web:**

```powershell
cd C:\Users\ADMIN\source\repos\LongNai139\Incense-Retail-Management-System
dotnet run --project SV22T1080045.Shop.Web --launch-profile https
```

**Cửa sổ 2 — Admin:**

```powershell
cd C:\Users\ADMIN\source\repos\LongNai139\Incense-Retail-Management-System
dotnet run --project SV22T1080045.Shop.Admin --launch-profile https
```

Hoặc ép URL (không phụ thuộc `launchSettings`):

```powershell
dotnet run --project SV22T1080045.Shop.Web --urls "https://0.0.0.0:7126;http://0.0.0.0:5299"
```

Điện thoại: `http://<IP-của-bạn>:5299`.

---

## 7. Checklist chức năng

### Cửa hàng (`SV22T1080045.Shop.Web`)

- [ ] Trang chủ, danh sách & chi tiết sản phẩm  
- [ ] Giỏ hàng, voucher, upsell  
- [ ] Checkout **COD** → trang thành công  
- [ ] Checkout **Chuyển khoản QR** (sau khi cấu hình `VietQr` trong appsettings)  
- [ ] Tra cứu đơn theo SĐT  
- [ ] Đăng ký / đăng nhập khách  

### Backoffice (`SV22T1080045.Shop.Admin`)

- [ ] `https://localhost:7127/Management` hoặc `http://<IP>:5298`  
- [ ] Staff — xử lý đơn, âm báo (bật **Âm báo**, tạo đơn mới từ Web)  
- [ ] Đăng nhập Admin/Staff (tài khoản seed — xem README / seed trong DB)

**Hai cửa sổ khi F5 (Web + Admin):**

| Cửa sổ | URL mở ra | Đăng nhập bằng |
|--------|-----------|----------------|
| Cửa hàng | `https://localhost:7126` | Tài khoản **khách** (mua hàng) |
| Quản trị | `https://localhost:7127/Account/Login` | **Staff** hoặc **Admin** |

- Cookie **không dùng chung** giữa 7126 và 7127 — mỗi cổng một cookie riêng (xem bảng mục [3.4](#34-checklist-phân-quyền-trên-một-máy-localhost)).
- Đăng nhập Staff/Admin trên **7126** → trang **bridge** POST sang `7127/Account/BridgeLogin`.
- **Đăng xuất đồng bộ:** 7126 (Staff/Admin) → `BackofficeLogoutBridge`; 7127 → `StorefrontLogoutBridge` + `7126/Account/LogoutBridge`.
- **Không** đăng nhập khách trên cổng 7127 — hệ thống báo và yêu cầu sang 7126.
- Tài khoản mẫu Admin (seed DB): SĐT `0909123456`, mật khẩu `123`. Staff (tự seed khi chạy Admin): `0911000001` / `123`, `0911000002` / `123`.

**Phân quyền:**

| Role | Cửa hàng (7126) | Staff (7127/Staff) | Quản lý (7127/Management) |
|------|-----------------|--------------------|---------------------------|
| Customer | ✅ Mua hàng, giỏ, tài khoản | ❌ | ❌ |
| Staff | ✅ Trang khách + đăng nhập Web | ✅ Xử lý đơn | ❌ |
| Admin | ✅ (mở tab mới từ nav) | ✅ | ✅ |

Gán role trong SQL: `UPDATE Customers SET Role = 'Staff' WHERE Phone = '09xxxxxxxx';` (chỉ `Customer`, `Staff`, `Admin`).

**Nav Management:** dùng lại nav cửa hàng (logo, tìm kiếm, Trang chủ…). Link cửa hàng mở **tab mới** (`7126`) — cổng 7127 vẫn giữ phiên quản trị.

**Hai cookie (quan trọng):** cửa hàng `7126` và quản trị `7127` dùng cookie **khác nhau**. Đăng nhập Admin trên `7126` sẽ tự chuyển sang `7127` qua bước “bridge” (vài giây) để tạo phiên quản trị. Nếu vào thẳng `https://localhost:7127/Management` mà **404** → kiểm tra project **Shop.Admin** đang chạy (Multiple startup projects), không chỉ Shop.Web.

**Khuyến nghị:** đăng nhập Staff/Admin trực tiếp tại `https://localhost:7127/Account/Login`. Dùng `7126` cho khách mua hàng.

**Staff vào `/Management`:** `[Authorize(Roles = Admin)]` chặn server-side; Staff thấy trang `/Account/AccessDenied` (không phải 404). Redirect `/` trên Admin theo role: Admin → `/Management`, Staff → `/Staff`.

---

## 8. Thanh toán VNPay / MoMo / QR

Cấu hình trong `SV22T1080045.Shop.Web\appsettings.json` (chỉ Web).

| Cổng | Bạn cần làm |
|------|----------------|
| **COD** | Không cấu hình thêm |
| **VietQR** | Điền `BankId`, `AccountNumber`, `AccountName` — tài khoản NH thật |
| **VNPay** | Đăng ký sandbox, điền `TmnCode`, `HashSecret`, Return URL |
| **MoMo** | Đăng ký developer, điền PartnerCode/AccessKey/SecretKey, Redirect + IPN URL |

**Return URL mẫu** (thay host bằng IP LAN hoặc ngrok):

```text
http://192.168.1.50:5299/Checkout/VnPayReturn
http://192.168.1.50:5299/Checkout/MoMoReturn
http://192.168.1.50:5299/Checkout/MoMoIpn
```

VNPay/MoMo sandbox **thường không chấp nhận IP LAN** `192.168.x.x` — khi test thanh toán online dùng **ngrok** (mục 9).

---

## 9. Ngrok (khi cần URL công khai)

Dùng khi: VNPay/MoMo callback, hoặc muốn HTTPS tin cậy trên điện thoại.

1. Cài [ngrok](https://ngrok.com/download), đăng ký tài khoản free.  
2. Chạy Web local port **5299** (HTTP):

```powershell
dotnet run --project SV22T1080045.Shop.Web --urls "http://0.0.0.0:5299"
```

3. Terminal khác:

```powershell
ngrok http 5299
```

4. Copy URL dạng `https://abcd-1234.ngrok-free.app`.  
5. Cập nhật:

```json
"AppHosts": {
  "StorefrontUrl": "https://abcd-1234.ngrok-free.app",
  "BackofficeUrl": "http://192.168.1.50:5298"
}
```

6. Khai báo Return URL trên VNPay/MoMo: `https://abcd-1234.ngrok-free.app/Checkout/VnPayReturn` (v.v.).

**Lưu ý:** URL ngrok đổi mỗi lần chạy (bản free) — phải cập nhật lại cổng thanh toán.

---

## 10. Xử lý lỗi thường gặp

### Điện thoại: “Không thể truy cập trang web” / quay mãi

| Nguyên nhân | Cách xử lý |
|-------------|------------|
| App chưa chạy | Chạy lại Web, xem log `Now listening on: http://0.0.0.0:5299` |
| Sai IP | `ipconfig` lại, dùng IPv4 của Wi‑Fi |
| Sai cổng | Cửa hàng = **5299** (HTTP), không phải 7126 trừ khi test HTTPS |
| Firewall | Làm lại bước C (PowerShell Admin) |
| Khác Wi‑Fi / 4G | Tắt 4G, cùng Wi‑Fi với PC |
| Guest Wi‑Fi | Đổi sang Wi‑Fi chính (không guest) |
| PC vào `http://IP:5299` cũng không được | Sửa `launchSettings` + restart app |
| PC vào được, điện thoại không | Firewall hoặc router cô lập LAN → thử hotspot từ điện thoại (PC kết nối hotspot điện thoại) |

### PC: cảnh báo chứng chỉ HTTPS

```powershell
dotnet dev-certs https --trust
```

Rồi restart trình duyệt. Điện thoại vẫn nên dùng HTTP `:5299`.

### Visual Studio: “Address already in use”

Port đang bị process cũ giữ:

```powershell
netstat -ano | findstr :5299
taskkill /PID <số_PID> /F
```

### Database / trang trắng lỗi 500

- Kiểm tra SQL Server đang chạy.  
- Chạy lại `dotnet ef database update`.  
- Xem log trong cửa sổ terminal khi F5.

### Link Staff/Quản lý trên Web sai

Chưa sửa `AppHosts` hoặc chưa restart sau bước F.

---

## Tóm tắt: Bạn đang ở đâu?

| Bước | Việc | Trạng thái gợi ý |
|------|------|------------------|
| A | Lấy IP (`ipconfig`) | Làm ngay |
| B | Sửa `launchSettings` `0.0.0.0` | ✅ Bạn đã làm (HTTPS) |
| C | Firewall | **Làm tiếp** — hay bị quên |
| D | Chạy app + test `http://IP:5299` trên PC | **Làm tiếp** |
| E | Mở `http://IP:5299` trên điện thoại | Sau C, D |
| F | Sửa `AppHosts` trong appsettings | Sau khi E chạy được |
| G | HTTPS điện thoại | Bỏ qua lúc đầu |
| H | Checklist chức năng | Cuối cùng |

**Thứ tự tối thiểu để điện thoại vào được:** `A → B (xong) → C → D (test PC bằng IP) → E`.

Nếu sau bước D mà trên PC `http://192.168.x.x:5299` vẫn không mở, gửi kèm: ảnh `ipconfig`, dòng log `Now listening on...`, và thông báo lỗi trình duyệt — để khoanh vùng firewall hay cấu hình listen.
