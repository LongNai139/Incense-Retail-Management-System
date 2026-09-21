-- Update password cho các tài khoản Admin và Staff có password ngắn hơn 8 ký tự
-- Script này cập nhật password mặc định cho các tài khoản quản trị

-- Cập nhật password cho Admin (mặc định: Admin@1234)
UPDATE Customers
SET Password = 'Admin@1234'
WHERE Role IN ('Admin', 'Staff')
  AND (LEN(Password) < 8 OR Password IS NULL)
  AND IsDeleted = 0;

-- Hoặc cập nhật password riêng cho từng role
-- Password cho Admin: Admin@1234
UPDATE Customers
SET Password = 'Admin@1234'
WHERE Role = 'Admin'
  AND (LEN(Password) < 8 OR Password IS NULL)
  AND IsDeleted = 0;

-- Password cho Staff: Staff@1234
UPDATE Customers
SET Password = 'Staff@1234'
WHERE Role = 'Staff'
  AND (LEN(Password) < 8 OR Password IS NULL)
  AND IsDeleted = 0;

-- Kiểm tra kết quả
SELECT Id, CustomerName, Phone, Role, Password, LEN(Password) as PasswordLength
FROM Customers
WHERE Role IN ('Admin', 'Staff')
  AND IsDeleted = 0
ORDER BY Role, CustomerName;