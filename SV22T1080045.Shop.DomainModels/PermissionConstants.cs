namespace SV22T1080045.Shop.DomainModels
{
    /// <summary>
    /// Định nghĩa các permission granular cho hệ thống
    /// Permission được đặt tên theo cấu trúc: action_resource
    /// Ví dụ: view_order, add_order, delete_order
    /// </summary>
    public static class PermissionConstants
    {
        // === QUẢN LÝ HỆ THỐNG (SYSTEM) ===
        public const string ManageSystem = "manage_system";          // Toàn quyền hệ thống (super admin)
        public const string ManagePermission = "manage_permission";  // Quản lý quyền (role, group)
        public const string ViewUser = "view_user";                  // Xem danh sách user
        public const string CreateUser = "create_user";              // Tạo user mới
        public const string EditUser = "edit_user";                  // Sửa user
        public const string DeleteUser = "delete_user";              // Xóa user
        public const string ViewLogs = "view_logs";                  // Xem logs hoạt động

        // === QUẢN LÝ ĐƠN HÀNG (ORDERS) ===
        public const string ViewOrder = "view_order";                // Xem danh sách đơn hàng
        public const string AddOrder = "add_order";                  // Tạo đơn hàng mới
        public const string EditOrder = "edit_order";                 // Sửa đơn hàng
        public const string DeleteOrder = "delete_order";            // Xóa đơn hàng
        public const string ApproveOrder = "approve_order";          // Duyệt/Xác nhận đơn hàng
        public const string ViewOrderDetails = "view_order_details";  // Xem chi tiết đơn hàng

        // === QUẢN LÝ SẢN PHẨM (PRODUCTS) ===
        public const string ViewProduct = "view_product";            // Xem danh sách sản phẩm
        public const string AddProduct = "add_product";              // Thêm sản phẩm mới
        public const string EditProduct = "edit_product";             // Sửa sản phẩm
        public const string DeleteProduct = "delete_product";        // Xóa sản phẩm
        public const string ManageCategory = "manage_category";      // Quản lý danh mục sản phẩm

        // === QUẢN LÝ KHÁCH HÀNG (CUSTOMERS) ===
        public const string ViewCustomer = "view_customer";          // Xem danh sách khách hàng
        public const string AddCustomer = "add_customer";            // Thêm khách hàng
        public const string EditCustomer = "edit_customer";          // Sửa khách hàng
        public const string DeleteCustomer = "delete_customer";      // Xóa khách hàng

        // === QUẢN LÝ KHO HÀNG (INVENTORY) ===
        public const string ViewInventory = "view_inventory";        // Xem tồn kho
        public const string ImportInventory = "import_inventory";    // Nhập hàng vào kho
        public const string ExportInventory = "export_inventory";    // Xuất hàng khỏi kho

        // === QUẢN LÝ NHÀ CUNG CẤP (SUPPLIERS) ===
        public const string ViewSupplier = "view_supplier";          // Xem danh sách nhà cung cấp
        public const string AddSupplier = "add_supplier";            // Thêm nhà cung cấp
        public const string EditSupplier = "edit_supplier";           // Sửa nhà cung cấp
        public const string DeleteSupplier = "delete_supplier";      // Xóa nhà cung cấp

        // === BÁO CÁO & THỐNG KÊ (REPORTS) ===
        public const string ViewReport = "view_report";              // Xem báo cáo
        public const string ViewRevenue = "view_revenue";            // Xem doanh thu
        public const string ViewProfit = "view_profit";              // Xem lợi nhuận
        public const string ExportReport = "export_report";          // Xuất báo cáo

        // === CÔNG NỢ (DEBT) ===
        public const string ViewDebt = "view_debt";                  // Xem công nợ
        public const string CollectPayment = "collect_payment";      // Thu tiền công nợ

        // === QUẢN LÝ GIẢM GIÁ (DISCOUNTS) ===
        public const string ManageVoucher = "manage_voucher";        // Quản lý mã giảm giá
        public const string CreateVoucher = "create_voucher";        // Tạo mã giảm giá
        public const string DeleteVoucher = "delete_voucher";        // Xóa mã giảm giá
    }
}
