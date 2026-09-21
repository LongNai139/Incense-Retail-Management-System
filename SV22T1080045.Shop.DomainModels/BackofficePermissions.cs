namespace SV22T1080045.Shop.DomainModels;

public static class BackofficePermissions
{
    public static readonly IReadOnlyList<RolePermissionDefinition> Definitions = new[]
    {
        new RolePermissionDefinition
        {
            Role = CustomerRoles.Admin,
            DisplayName = "Quản trị viên",
            Description = "Toàn quyền quản lý cửa hàng, hệ thống và phân quyền.",
            Permissions = new[]
            {
                "Quản lý sản phẩm, đơn hàng, khách hàng, mã giảm giá",
                "Truy cập khu vực Staff và Management",
                "Quản lý tài khoản và phân quyền (Hệ thống)",
                "Đăng nhập cửa hàng và quản trị"
            }
        },
        new RolePermissionDefinition
        {
            Role = CustomerRoles.Staff,
            DisplayName = "Nhân viên",
            Description = "Xử lý đơn hàng và theo dõi tồn kho trong ngày.",
            Permissions = new[]
            {
                "Truy cập khu vực Staff",
                "Cập nhật trạng thái đơn hàng",
                "Xem sản phẩm và cảnh báo tồn kho",
                "Đăng nhập cửa hàng và quản trị"
            }
        },
        new RolePermissionDefinition
        {
            Role = CustomerRoles.Customer,
            DisplayName = "Khách hàng",
            Description = "Mua hàng và theo dõi đơn trên cửa hàng.",
            Permissions = new[]
            {
                "Mua hàng, giỏ hàng, thanh toán",
                "Tra cứu và theo dõi đơn hàng",
                "Chỉ đăng nhập tại cửa hàng (7126)"
            }
        }
    };
}

public class RolePermissionDefinition
{
    public string Role { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string Description { get; set; } = "";
    public IReadOnlyList<string> Permissions { get; set; } = Array.Empty<string>();
}
