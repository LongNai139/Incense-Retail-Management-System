# HƯỚNG DẪN MIGRATION HỆ THỐNG PHÂN QUYỀN

## Tổng quan

Migration này thêm hệ thống phân quyền chi tiết vào dự án, thay thế cơ chế hard-code Role string bằng database-driven permission system.

## Các thay đổi

### Database Tables
- **Roles**: Bảng lưu các role (Admin, Staff, Customer)
- **Permissions**: Bảng lưu các permission nghiệp vụ (view_order, add_order, etc.)
- **RolePermissions**: Bảng many-to-many giữa Roles và Permissions
- **Groups**: Bảng nhóm các Role lại (optional)
- **RoleGroups**: Bảng many-to-many giữa Groups và Roles
- **UserPermissions**: Bảng gán permission riêng cho User (optional)

### Bảng Customers
- Thêm cột `RoleId` (FK đến Roles)
- Thêm cột `GroupId` (FK đến Groups, optional)
- Giữ lại cột `Role` (string) để backward compatibility, sẽ xóa sau migration hoàn tất

### C# Models
- Thêm `Role.cs`
- Thêm `Permission.cs`
- Thêm `RolePermission.cs`
- Thêm `Group.cs`
- Thadd `RoleGroup.cs`
- Thêm `UserPermission.cs`
- Cập nhật `Customer.cs` để thêm navigation properties
- Cập nhật `ShopDbContext.cs` để thêm DbSets và configuration
- Đánh dấu `CustomerRoles.RolePermissions` là Obsolete

## Cách chạy Migration

### Bước 1: Backup Database

**QUAN TRỌNG:** Luôn backup database trước khi chạy migration!

```sql
-- Backup database
BACKUP DATABASE ShopIncenseDB 
TO DISK = 'C:\Backup\ShopIncenseDB_BeforePermissionMigration_' + CONVERT(VARCHAR, GETDATE(), 112) + '.bak'
WITH FORMAT, MEDIANAME = 'ShopIncenseDB_Full', NAME = 'Full Backup of ShopIncenseDB';
```

Hoặc sử dụng SQL Server Management Studio:
1. Right-click database → Tasks → Back Up
2. Chọn destination folder
3. Click OK

### Bước 2: Chạy Migration Script

Mở SQL Server Management Studio (SSMS) và chạy file:

```
SV22T1080045.Shop.DataLayers\Scripts\Migration_AddPermissionSystem.sql
```

**Hoặc chạy bằng command line:**

```bash
sqlcmd -S . -d ShopIncenseDB -i Migration_AddPermissionSystem.sql
```

### Bước 3: Verify Migration

Sau khi chạy script, kiểm tra kết quả:

```sql
-- Kiểm tra các bảng mới đã tạo
SELECT name FROM sys.tables WHERE name IN ('Roles', 'Permissions', 'RolePermissions', 'Groups', 'RoleGroups', 'UserPermissions');

-- Kiểm tra dữ liệu Roles
SELECT * FROM Roles;

-- Kiểm tra dữ liệu Permissions
SELECT * FROM Permissions;

-- Kiểm tra dữ liệu RolePermissions
SELECT * FROM RolePermissions;

-- Kiểm tra migration Customers
SELECT 
    Id, 
    CustomerName, 
    Phone, 
    Role AS OldRole, 
    RoleId AS NewRoleId,
    GroupId
FROM Customers 
WHERE IsDeleted = 0;
```

### Bước 4: Kiểm tra Data Integrity

```sql
-- Đảm bảo tất cả Customers đã được gán RoleId
SELECT COUNT(*) AS CustomersWithoutRoleId
FROM Customers
WHERE IsDeleted = 0 AND RoleId IS NULL;

-- Nếu có Customer không có RoleId, chạy thủ công:
UPDATE Customers
SET RoleId = (SELECT Id FROM Roles WHERE Name = 'Customer')
WHERE IsDeleted = 0 AND RoleId IS NULL;
```

## Dữ liệu được seed

### Roles
- **Admin** (IsSystem = true): Quản trị viên
- **Staff** (IsSystem = true): Nhân viên
- **Customer** (IsSystem = true): Khách hàng

### Permissions (63 permissions)
- **System**: manage_system, manage_permission, view_user, create_user, edit_user, delete_user, reset_password, view_logs
- **Orders**: view_order, view_order_details, add_order, edit_order, delete_order, approve_order, cancel_order, update_order_status, export_order
- **Products**: view_product, add_product, edit_product, delete_product, manage_category, manage_unit
- **Customers**: view_customer, add_customer, edit_customer, delete_customer, view_customer_history
- **Inventory**: view_inventory, import_inventory, export_inventory, adjust_inventory, view_stock_alert
- **Suppliers**: view_supplier, add_supplier, edit_supplier, delete_supplier
- **Reports**: view_report, view_revenue, view_profit, export_report, view_dashboard
- **Debt**: view_debt, collect_payment, manage_debt
- **Discounts**: manage_voucher, create_voucher, edit_voucher, delete_voucher, activate_voucher
- **Payments**: view_payment, process_payment, refund_payment

### RolePermissions
- **Admin**: Có tất cả 63 permissions
- **Staff**: Có 9 permissions (view_order, view_order_details, add_order, approve_order, update_order_status, view_product, view_customer, view_inventory, view_stock_alert)
- **Customer**: Không có permission đặc biệt

### Groups
- **Quản trị viên** (IsSystem = true): Chứa Role Admin
- **Nhân viên bán hàng** (IsSystem = true): Chứa Role Staff
- **Nhân viên kho** (IsSystem = true): Chứa Role Staff

## Rollback (Nếu cần)

Nếu migration có lỗi và cần rollback:

```sql
-- Drop các bảng mới (theo đúng thứ tự do foreign key)
DROP TABLE IF EXISTS UserPermissions;
DROP TABLE IF EXISTS RoleGroups;
DROP TABLE IF EXISTS Groups;
DROP TABLE IF EXISTS RolePermissions;
DROP TABLE IF EXISTS Permissions;
DROP TABLE IF EXISTS Roles;

-- Xóa cột mới trong Customers
ALTER TABLE Customers DROP CONSTRAINT IF EXISTS FK_Customers_Group;
ALTER TABLE Customers DROP CONSTRAINT IF EXISTS FK_Customers_Role;
ALTER TABLE Customers DROP COLUMN IF EXISTS GroupId;
ALTER TABLE Customers DROP COLUMN IF EXISTS RoleId;
```

Sau đó restore database từ backup.

## Các bước tiếp theo

Sau khi migration database thành công, cần:

1. **Giai đoạn 2**: Implement Data Layer (DAL cho Roles, Permissions, etc.)
2. **Giai đoạn 3**: Implement Business Layer (PermissionService)
3. **Giai đoạn 4**: Cập nhật Authentication để load permissions từ database
4. **Giai đoạn 5**: Áp dụng Permission Attribute cho Controllers
5. **Giai đoạn 6**: Xây dựng giao diện quản lý quyền
6. **Giai đoạn 7**: Cập nhật UI để ẩn/hiện theo permission
7. **Giai đoạn 8**: Rà soát toàn bộ endpoint
8. **Giai đoạn 9**: Kiểm thử và sửa lỗi
9. **Giai đoạn 10**: Deployment và Monitoring

## Lưu ý quan trọng

1. **Không được xóa cột `Role` trong Customers** cho đến khi tất cả code đã được cập nhật để dùng `RoleId`
2. **Không được xóa `CustomerRoles.RolePermissions` dictionary** cho đến khi migration hoàn tất
3. **Test kỹ trên database test trước khi chạy trên production**
4. **Monitor logs sau khi migration để phát hiện error**

## Support

Nếu gặp lỗi trong quá trình migration:
1. Kiểm tra SQL Server logs
2. Kiểm tra lỗi trong output của script
3. Restore từ backup và thử lại
4. Liên hệ developer hỗ trợ

---

**Ngày tạo:** 2025-01-XX  
**Phiên bản:** 1.0  
**Tác giả:** System
