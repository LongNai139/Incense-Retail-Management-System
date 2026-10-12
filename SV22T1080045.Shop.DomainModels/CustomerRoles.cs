namespace SV22T1080045.Shop.DomainModels;

public static class CustomerRoles
{
    public const string Customer = "Customer";
    public const string Staff = "Staff";
    public const string Admin = "Admin";

    /// <summary>Quyền quản lý module Hệ thống — tương đương Admin trong dự án hiện tại.</summary>
    public const string ManageSystem = Admin;

    public static readonly string[] BackofficeRoles = { Admin, Staff };
    public static readonly string[] AllRoles = { Customer, Staff, Admin };

    /// <summary>
    /// Map các role với permission tương ứng
    /// Admin: Tất cả quyền
    /// Staff: Các quyền dành cho nhân viên
    /// Customer: Chỉ mua hàng
    /// </summary>
    public static readonly Dictionary<string, string[]> RolePermissions = new()
    {
        {
            Admin, new[]
            {
                // Admin có tất cả quyền
                PermissionConstants.ManageSystem,
                PermissionConstants.ManagePermission,
                PermissionConstants.ViewUser,
                PermissionConstants.CreateUser,
                PermissionConstants.EditUser,
                PermissionConstants.DeleteUser,
                PermissionConstants.ViewLogs,
                PermissionConstants.ViewOrder,
                PermissionConstants.AddOrder,
                PermissionConstants.EditOrder,
                PermissionConstants.DeleteOrder,
                PermissionConstants.ApproveOrder,
                PermissionConstants.ViewOrderDetails,
                PermissionConstants.ViewProduct,
                PermissionConstants.AddProduct,
                PermissionConstants.EditProduct,
                PermissionConstants.DeleteProduct,
                PermissionConstants.ManageCategory,
                PermissionConstants.ViewCustomer,
                PermissionConstants.AddCustomer,
                PermissionConstants.EditCustomer,
                PermissionConstants.DeleteCustomer,
                PermissionConstants.ViewInventory,
                PermissionConstants.ImportInventory,
                PermissionConstants.ExportInventory,
                PermissionConstants.ViewSupplier,
                PermissionConstants.AddSupplier,
                PermissionConstants.EditSupplier,
                PermissionConstants.DeleteSupplier,
                PermissionConstants.ViewReport,
                PermissionConstants.ViewRevenue,
                PermissionConstants.ViewProfit,
                PermissionConstants.ExportReport,
                PermissionConstants.ViewDebt,
                PermissionConstants.CollectPayment,
                PermissionConstants.ManageVoucher,
                PermissionConstants.CreateVoucher,
                PermissionConstants.DeleteVoucher
            }
        },
        {
            Staff, new[]
            {
                // Staff có quyền xem đơn, xử lý đơn, xem sản phẩm, xem khách hàng
                PermissionConstants.ViewOrder,
                PermissionConstants.AddOrder,
                PermissionConstants.ApproveOrder,
                PermissionConstants.ViewOrderDetails,
                PermissionConstants.ViewProduct,
                PermissionConstants.ViewCustomer,
                PermissionConstants.ViewInventory
            }
        },
        {
            Customer, new string[] { }
            // Customer không có permission đặc biệt, chỉ được mua hàng
        }
    };
}
