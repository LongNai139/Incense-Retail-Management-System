-- ============================================================================
-- MIGRATION: THÊM HỆ THỐNG PHÂN QUYỀN (ROLES, PERMISSIONS, GROUPS)
-- Database: ShopIncenseDB
-- Date: 2025-01-XX
-- Author: System
-- ============================================================================

-- ============================================================================
-- PHẦN 1: TẠO CÁC BẢNG MỚI CHO HỆ THỐNG PHÂN QUYỀN
-- ============================================================================

-- 1.1 Bảng Roles (Thay thế hard-code string)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Roles')
BEGIN
    CREATE TABLE Roles (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(50) UNIQUE NOT NULL,  -- 'Admin', 'Staff', 'Customer'
        DisplayName NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500),
        IsSystem BIT DEFAULT 0,  -- Không cho xóa role hệ thống
        CreatedAt DATETIME2 DEFAULT GETDATE(),
        UpdatedAt DATETIME2 DEFAULT GETDATE(),
        CONSTRAINT CK_Roles_Name_NotEmpty CHECK (LEN(Name) > 0)
    );
    
    PRINT '✓ Created table Roles';
END
ELSE
BEGIN
    PRINT '✗ Table Roles already exists';
END
GO

-- 1.2 Bảng Permissions
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Permissions')
BEGIN
    CREATE TABLE Permissions (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Code NVARCHAR(100) UNIQUE NOT NULL,  -- 'view_order', 'add_order', ...
        DisplayName NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500),
        Module NVARCHAR(50),  -- 'Orders', 'Products', 'Customers', ...
        CreatedAt DATETIME2 DEFAULT GETDATE(),
        CONSTRAINT CK_Permissions_Code_NotEmpty CHECK (LEN(Code) > 0)
    );
    
    PRINT '✓ Created table Permissions';
END
ELSE
BEGIN
    PRINT '✗ Table Permissions already exists';
END
GO

-- 1.3 Bảng RolePermissions (Many-to-Many giữa Roles và Permissions)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RolePermissions')
BEGIN
    CREATE TABLE RolePermissions (
        RoleId INT NOT NULL,
        PermissionId INT NOT NULL,
        GrantedAt DATETIME2 DEFAULT GETDATE(),
        GrantedBy INT NULL,  -- User ID người cấp quyền
        PRIMARY KEY (RoleId, PermissionId),
        CONSTRAINT FK_RolePermissions_Role FOREIGN KEY (RoleId) 
            REFERENCES Roles(Id) ON DELETE CASCADE,
        CONSTRAINT FK_RolePermissions_Permission FOREIGN KEY (PermissionId) 
            REFERENCES Permissions(Id) ON DELETE CASCADE
    );
    
    PRINT '✓ Created table RolePermissions';
END
ELSE
BEGIN
    PRINT '✗ Table RolePermissions already exists';
END
GO

-- 1.4 Bảng Groups (Nhóm các Role lại - giống dự án tham chiếu)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Groups')
BEGIN
    CREATE TABLE Groups (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) UNIQUE NOT NULL,
        Description NVARCHAR(500),
        IsSystem BIT DEFAULT 0,
        CreatedAt DATETIME2 DEFAULT GETDATE(),
        UpdatedAt DATETIME2 DEFAULT GETDATE(),
        CONSTRAINT CK_Groups_Name_NotEmpty CHECK (LEN(Name) > 0)
    );
    
    PRINT '✓ Created table Groups';
END
ELSE
BEGIN
    PRINT '✗ Table Groups already exists';
END
GO

-- 1.5 Bảng RoleGroups (Many-to-Many giữa Groups và Roles)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RoleGroups')
BEGIN
    CREATE TABLE RoleGroups (
        GroupId INT NOT NULL,
        RoleId INT NOT NULL,
        PRIMARY KEY (GroupId, RoleId),
        CONSTRAINT FK_RoleGroups_Group FOREIGN KEY (GroupId) 
            REFERENCES Groups(Id) ON DELETE CASCADE,
        CONSTRAINT FK_RoleGroups_Role FOREIGN KEY (RoleId) 
            REFERENCES Roles(Id) ON DELETE CASCADE
    );
    
    PRINT '✓ Created table RoleGroups';
END
ELSE
BEGIN
    PRINT '✗ Table RoleGroups already exists';
END
GO

-- 1.6 Bảng UserPermissions (Gán permission riêng cho User - OPTIONAL)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserPermissions')
BEGIN
    CREATE TABLE UserPermissions (
        UserId INT NOT NULL,
        PermissionId INT NOT NULL,
        IsGranted BIT NOT NULL DEFAULT 1,  -- true = cấp quyền, false = thu hồi quyền
        GrantedAt DATETIME2 DEFAULT GETDATE(),
        GrantedBy INT NULL,  -- User ID người cấp quyền
        ExpiresAt DATETIME2 NULL,  -- Quyền tạm thời
        PRIMARY KEY (UserId, PermissionId),
        CONSTRAINT FK_UserPermissions_User FOREIGN KEY (UserId) 
            REFERENCES Customers(Id) ON DELETE CASCADE,
        CONSTRAINT FK_UserPermissions_Permission FOREIGN KEY (PermissionId) 
            REFERENCES Permissions(Id) ON DELETE CASCADE
    );
    
    PRINT '✓ Created table UserPermissions';
END
ELSE
BEGIN
    PRINT '✗ Table UserPermissions already exists';
END
GO

-- ============================================================================
-- PHẦN 2: CẬP NHẬT BẢNG CUSTOMERS
-- ============================================================================

-- 2.1 Thêm cột RoleId vào bảng Customers
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID('Customers') AND name = 'RoleId'
)
BEGIN
    ALTER TABLE Customers
    ADD RoleId INT NULL;
    
    PRINT '✓ Added column RoleId to Customers';
END
ELSE
BEGIN
    PRINT '✗ Column RoleId already exists in Customers';
END
GO

-- 2.2 Thêm Foreign Key cho RoleId
IF NOT EXISTS (
    SELECT * FROM sys.foreign_keys 
    WHERE object_id = OBJECT_ID('FK_Customers_Role') AND parent_object_id = OBJECT_ID('Customers')
)
BEGIN
    ALTER TABLE Customers
    ADD CONSTRAINT FK_Customers_Role FOREIGN KEY (RoleId) 
        REFERENCES Roles(Id);
    
    PRINT '✓ Added FK_Customers_Role foreign key';
END
ELSE
BEGIN
    PRINT '✗ FK_Customers_Role already exists';
END
GO

-- 2.3 Thêm cột GroupId vào bảng Customers (OPTIONAL - nếu muốn dùng Group)
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID('Customers') AND name = 'GroupId'
)
BEGIN
    ALTER TABLE Customers
    ADD GroupId INT NULL;
    
    PRINT '✓ Added column GroupId to Customers';
END
ELSE
BEGIN
    PRINT '✗ Column GroupId already exists in Customers';
END
GO

-- 2.4 Thêm Foreign Key cho GroupId
IF NOT EXISTS (
    SELECT * FROM sys.foreign_keys 
    WHERE object_id = OBJECT_ID('FK_Customers_Group') AND parent_object_id = OBJECT_ID('Customers')
)
BEGIN
    ALTER TABLE Customers
    ADD CONSTRAINT FK_Customers_Group FOREIGN KEY (GroupId) 
        REFERENCES Groups(Id);
    
    PRINT '✓ Added FK_Customers_Group foreign key';
END
ELSE
BEGIN
    PRINT '✗ FK_Customers_Group already exists';
END
GO

-- ============================================================================
-- PHẦN 3: CHUYỂN ĐỔI DỮ LIỆU TỪ ROLE STRING SANG ROLEID
-- ============================================================================

-- 3.1 Seed default roles
IF NOT EXISTS (SELECT * FROM Roles WHERE Name = 'Admin')
BEGIN
    INSERT INTO Roles (Name, DisplayName, Description, IsSystem)
    VALUES 
        ('Admin', N'Quản trị viên', N'Toàn quyền quản lý cửa hàng, hệ thống và phân quyền', 1),
        ('Staff', N'Nhân viên', N'Xử lý đơn hàng và theo dõi tồn kho trong ngày', 1),
        ('Customer', N'Khách hàng', N'Mua hàng và theo dõi đơn trên cửa hàng', 1);
    
    PRINT '✓ Seeded default roles';
END
ELSE
BEGIN
    PRINT '✗ Default roles already exist';
END
GO

-- 3.2 Chuyển đổi dữ liệu: Role string → RoleId
UPDATE Customers
SET RoleId = (
    CASE 
        WHEN Role = 'Admin' THEN (SELECT Id FROM Roles WHERE Name = 'Admin')
        WHEN Role = 'Staff' THEN (SELECT Id FROM Roles WHERE Name = 'Staff')
        WHEN Role = 'Customer' THEN (SELECT Id FROM Roles WHERE Name = 'Customer')
        ELSE NULL
    END
)
WHERE RoleId IS NULL;

PRINT '✓ Migrated Role string to RoleId';
GO

-- 3.3 Verify migration
SELECT 
    COUNT(*) AS TotalCustomers,
    SUM(CASE WHEN RoleId IS NOT NULL THEN 1 ELSE 0 END) AS MigratedCount,
    SUM(CASE WHEN RoleId IS NULL THEN 1 ELSE 0 END) AS NotMigratedCount
FROM Customers
WHERE IsDeleted = 0;

PRINT '✓ Migration verification completed';
GO

-- ============================================================================
-- PHẦN 4: SEED DEFAULT PERMISSIONS
-- ============================================================================

-- 4.1 Seed permissions cho từng module
IF NOT EXISTS (SELECT * FROM Permissions WHERE Code = 'manage_system')
BEGIN
    INSERT INTO Permissions (Code, DisplayName, Description, Module)
    VALUES 
        -- HỆ THỐNG (System)
        ('manage_system', N'Quản lý hệ thống', N'Toàn quyền hệ thống (super admin)', 'System'),
        ('manage_permission', N'Quản lý quyền', N'Quản lý quyền (role, group)', 'System'),
        ('view_user', N'Xem người dùng', N'Xem danh sách user', 'System'),
        ('create_user', N'Tạo người dùng', N'Tạo user mới', 'System'),
        ('edit_user', N'Sửa người dùng', N'Sửa user', 'System'),
        ('delete_user', N'Xóa người dùng', N'Xóa user', 'System'),
        ('reset_password', N'Reset mật khẩu', N'Reset mật khẩu user', 'System'),
        ('view_logs', N'Xem logs', N'Xem logs hoạt động', 'System'),
        
        -- ĐƠN HÀNG (Orders)
        ('view_order', N'Xem đơn hàng', N'Xem danh sách đơn hàng', 'Orders'),
        ('view_order_details', N'Xem chi tiết đơn', N'Xem chi tiết đơn hàng', 'Orders'),
        ('add_order', N'Tạo đơn hàng', N'Tạo đơn hàng mới', 'Orders'),
        ('edit_order', N'Sửa đơn hàng', N'Sửa đơn hàng', 'Orders'),
        ('delete_order', N'Xóa đơn hàng', N'Xóa đơn hàng', 'Orders'),
        ('approve_order', N'Duyệt đơn hàng', N'Duyệt/Xác nhận đơn hàng', 'Orders'),
        ('cancel_order', N'Hủy đơn hàng', N'Hủy đơn hàng', 'Orders'),
        ('update_order_status', N'Cập nhật trạng thái', N'Cập nhật trạng thái đơn hàng', 'Orders'),
        ('export_order', N'Xuất đơn hàng', N'Xuất danh sách đơn hàng', 'Orders'),
        
        -- SẢN PHẨM (Products)
        ('view_product', N'Xem sản phẩm', N'Xem danh sách sản phẩm', 'Products'),
        ('add_product', N'Thêm sản phẩm', N'Thêm sản phẩm mới', 'Products'),
        ('edit_product', N'Sửa sản phẩm', N'Sửa sản phẩm', 'Products'),
        ('delete_product', N'Xóa sản phẩm', N'Xóa sản phẩm', 'Products'),
        ('manage_category', N'Quản lý danh mục', N'Quản lý danh mục sản phẩm', 'Products'),
        ('manage_unit', N'Quản lý đơn vị', N'Quản lý đơn vị tính', 'Products'),
        
        -- KHÁCH HÀNG (Customers)
        ('view_customer', N'Xem khách hàng', N'Xem danh sách khách hàng', 'Customers'),
        ('add_customer', N'Thêm khách hàng', N'Thêm khách hàng', 'Customers'),
        ('edit_customer', N'Sửa khách hàng', N'Sửa khách hàng', 'Customers'),
        ('delete_customer', N'Xóa khách hàng', N'Xóa khách hàng', 'Customers'),
        ('view_customer_history', N'Xem lịch sử', N'Xem lịch sử mua hàng', 'Customers'),
        
        -- KHO HÀNG (Inventory)
        ('view_inventory', N'Xem tồn kho', N'Xem tồn kho', 'Inventory'),
        ('import_inventory', N'Nhập kho', N'Nhập hàng vào kho', 'Inventory'),
        ('export_inventory', N'Xuất kho', N'Xuất hàng khỏi kho', 'Inventory'),
        ('adjust_inventory', N'Điều chỉnh kho', N'Điều chỉnh tồn kho', 'Inventory'),
        ('view_stock_alert', N'Xem cảnh báo', N'Xem cảnh báo tồn kho', 'Inventory'),
        
        -- NHÀ CUNG CẤP (Suppliers)
        ('view_supplier', N'Xem nhà cung cấp', N'Xem danh sách nhà cung cấp', 'Suppliers'),
        ('add_supplier', N'Thêm nhà cung cấp', N'Thêm nhà cung cấp', 'Suppliers'),
        ('edit_supplier', N'Sửa nhà cung cấp', N'Sửa nhà cung cấp', 'Suppliers'),
        ('delete_supplier', N'Xóa nhà cung cấp', N'Xóa nhà cung cấp', 'Suppliers'),
        
        -- BÁO CÁO (Reports)
        ('view_report', N'Xem báo cáo', N'Xem báo cáo', 'Reports'),
        ('view_revenue', N'Xem doanh thu', N'Xem doanh thu', 'Reports'),
        ('view_profit', N'Xem lợi nhuận', N'Xem lợi nhuận', 'Reports'),
        ('export_report', N'Xuất báo cáo', N'Xuất báo cáo', 'Reports'),
        ('view_dashboard', N'Xem dashboard', N'Xem dashboard', 'Reports'),
        
        -- CÔNG NỢ (Debt)
        ('view_debt', N'Xem công nợ', N'Xem công nợ', 'Debt'),
        ('collect_payment', N'Thu tiền', N'Thu tiền công nợ', 'Debt'),
        ('manage_debt', N'Quản lý công nợ', N'Quản lý công nợ', 'Debt'),
        
        -- GIẢM GIÁ (Discounts)
        ('manage_voucher', N'Quản lý voucher', N'Quản lý mã giảm giá', 'Discounts'),
        ('create_voucher', N'Tạo voucher', N'Tạo mã giảm giá', 'Discounts'),
        ('edit_voucher', N'Sửa voucher', N'Sửa mã giảm giá', 'Discounts'),
        ('delete_voucher', N'Xóa voucher', N'Xóa mã giảm giá', 'Discounts'),
        ('activate_voucher', N'Kích hoạt voucher', N'Kích hoạt/vô hiệu hóa voucher', 'Discounts'),
        
        -- THANH TOÁN (Payments)
        ('view_payment', N'Xem thanh toán', N'Xem thanh toán', 'Payments'),
        ('process_payment', N'Xử lý thanh toán', N'Xử lý thanh toán', 'Payments'),
        ('refund_payment', N'Hoàn tiền', N'Hoàn tiền', 'Payments');
    
    PRINT '✓ Seeded default permissions';
END
ELSE
BEGIN
    PRINT '✗ Default permissions already exist';
END
GO

-- ============================================================================
-- PHẦN 5: GÁN PERMISSION CHO ROLES (ROLEPERMISSIONS)
-- ============================================================================

-- 5.1 Admin có tất cả quyền
IF NOT EXISTS (SELECT * FROM RolePermissions WHERE RoleId = (SELECT Id FROM Roles WHERE Name = 'Admin'))
BEGIN
    INSERT INTO RolePermissions (RoleId, PermissionId, GrantedBy)
    SELECT 
        (SELECT Id FROM Roles WHERE Name = 'Admin'),
        Id,
        1  -- Granted by Admin (ID = 1)
    FROM Permissions;
    
    PRINT '✓ Granted all permissions to Admin role';
END
ELSE
BEGIN
    PRINT '✗ Admin permissions already granted';
END
GO

-- 5.2 Staff có quyền hạn chế
IF NOT EXISTS (
    SELECT * FROM RolePermissions 
    WHERE RoleId = (SELECT Id FROM Roles WHERE Name = 'Staff') 
    AND PermissionId = (SELECT Id FROM Permissions WHERE Code = 'view_order')
)
BEGIN
    INSERT INTO RolePermissions (RoleId, PermissionId, GrantedBy)
    SELECT 
        (SELECT Id FROM Roles WHERE Name = 'Staff'),
        Id,
        1
    FROM Permissions
    WHERE Code IN (
        'view_order', 'view_order_details', 'add_order', 'approve_order', 
        'update_order_status', 'view_product', 'view_customer', 'view_inventory',
        'view_stock_alert'
    );
    
    PRINT '✓ Granted limited permissions to Staff role';
END
ELSE
BEGIN
    PRINT '✗ Staff permissions already granted';
END
GO

-- 5.3 Customer không có permission đặc biệt
-- Customer chỉ được mua hàng, không cần permission
PRINT '✓ Customer role has no special permissions (as expected)';
GO

-- ============================================================================
-- PHẦN 6: SEED DEFAULT GROUPS (OPTIONAL)
-- ============================================================================

IF NOT EXISTS (SELECT * FROM Groups WHERE Name = N'Quản trị viên')
BEGIN
    INSERT INTO Groups (Name, Description, IsSystem)
    VALUES 
        (N'Quản trị viên', N'Nhóm quyền dành cho quản trị viên toàn hệ thống', 1),
        (N'Nhân viên bán hàng', N'Nhóm quyền dành cho nhân viên xử lý đơn hàng', 1),
        (N'Nhân viên kho', N'Nhóm quyền dành cho nhân viên quản lý kho', 1);
    
    PRINT '✓ Seeded default groups';
END
ELSE
BEGIN
    PRINT '✗ Default groups already exist';
END
GO

-- 6.1 Gán Role vào Groups
IF NOT EXISTS (SELECT * FROM RoleGroups WHERE GroupId = (SELECT Id FROM Groups WHERE Name = N'Quản trị viên'))
BEGIN
    INSERT INTO RoleGroups (GroupId, RoleId)
    SELECT 
        (SELECT Id FROM Groups WHERE Name = N'Quản trị viên'),
        (SELECT Id FROM Roles WHERE Name = 'Admin')
    UNION ALL
    SELECT 
        (SELECT Id FROM Groups WHERE Name = N'Nhân viên bán hàng'),
        (SELECT Id FROM Roles WHERE Name = 'Staff')
    UNION ALL
    SELECT 
        (SELECT Id FROM Groups WHERE Name = N'Nhân viên kho'),
        (SELECT Id FROM Roles WHERE Name = 'Staff');
    
    PRINT '✓ Assigned roles to groups';
END
ELSE
BEGIN
    PRINT '✗ Roles already assigned to groups';
END
GO

-- ============================================================================
-- PHẦN 7: TẠO INDEX ĐỂ TỐI ƯU PERFORMANCE
-- ============================================================================

-- Index cho Permissions
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Permissions_Module' AND object_id = OBJECT_ID('Permissions'))
BEGIN
    CREATE INDEX IX_Permissions_Module ON Permissions(Module);
    PRINT '✓ Created index IX_Permissions_Module';
END
GO

-- Index cho RolePermissions
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RolePermissions_PermissionId' AND object_id = OBJECT_ID('RolePermissions'))
BEGIN
    CREATE INDEX IX_RolePermissions_PermissionId ON RolePermissions(PermissionId);
    PRINT '✓ Created index IX_RolePermissions_PermissionId';
END
GO

-- Index cho UserPermissions
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UserPermissions_PermissionId' AND object_id = OBJECT_ID('UserPermissions'))
BEGIN
    CREATE INDEX IX_UserPermissions_PermissionId ON UserPermissions(PermissionId);
    PRINT '✓ Created index IX_UserPermissions_PermissionId';
END
GO

-- Index cho Customers.RoleId
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Customers_RoleId' AND object_id = OBJECT_ID('Customers'))
BEGIN
    CREATE INDEX IX_Customers_RoleId ON Customers(RoleId);
    PRINT '✓ Created index IX_Customers_RoleId';
END
GO

-- ============================================================================
-- PHẦN 8: VERIFY MIGRATION
-- ============================================================================

PRINT '===========================================';
PRINT 'MIGRATION VERIFICATION';
PRINT '===========================================';

SELECT 'Roles' AS TableName, COUNT(*) AS RecordCount FROM Roles
UNION ALL
SELECT 'Permissions', COUNT(*) FROM Permissions
UNION ALL
SELECT 'RolePermissions', COUNT(*) FROM RolePermissions
UNION ALL
SELECT 'Groups', COUNT(*) FROM Groups
UNION ALL
SELECT 'RoleGroups', COUNT(*) FROM RoleGroups
UNION ALL
SELECT 'UserPermissions', COUNT(*) FROM UserPermissions;

PRINT '===========================================';
PRINT 'CUSTOMER MIGRATION STATUS';
PRINT '===========================================';

SELECT 
    Role AS OldRole,
    RoleId AS NewRoleId,
    COUNT(*) AS CustomerCount
FROM Customers
WHERE IsDeleted = 0
GROUP BY Role, RoleId
ORDER BY Role;

PRINT '===========================================';
PRINT '✓ MIGRATION COMPLETED SUCCESSFULLY';
PRINT '===========================================';
GO
