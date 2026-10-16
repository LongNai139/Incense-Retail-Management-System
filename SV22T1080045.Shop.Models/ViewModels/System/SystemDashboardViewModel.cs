using System.ComponentModel.DataAnnotations;

namespace SV22T1080045.Shop.Models.ViewModels.System;

public class SystemDashboardViewModel
{
    public string AccountSearchValue { get; set; } = "";
    public string AccountRoleFilter { get; set; } = "";
    public string RoleSearchValue { get; set; } = "";
    public string RoleFilter { get; set; } = "";
    public int? EditAccountId { get; set; }
    public SystemAccountFormViewModel AccountForm { get; set; } = new();
    public List<SystemAccountRowViewModel> BackofficeAccounts { get; set; } = new();
    public List<SystemAccountRowViewModel> RoleAccounts { get; set; } = new();
    public List<SystemRoleAdminRowViewModel> PermissionRoles { get; set; } = new();
    public List<SystemPermissionRowViewModel> PermissionRows { get; set; } = new();
    public List<SystemUserOptionViewModel> PermissionUsers { get; set; } = new();
    public List<SystemRoleOptionViewModel> RoleOptions { get; set; } = new();
    public int? SelectedPermissionRoleId { get; set; }
    public int? SelectedPermissionUserId { get; set; }
    public SystemRoleFormViewModel RoleForm { get; set; } = new();
    public SystemPermissionFormViewModel PermissionForm { get; set; } = new();
    public int CurrentUserId { get; set; }
    public bool CanManagePermissions { get; set; }
    public bool CanViewManagement { get; set; }
    public bool CanCreateUsers { get; set; }
    public bool CanEditUsers { get; set; }
    public bool CanResetPasswords { get; set; }
    
    // Role constants for view
    public string AdminRole { get; set; } = "Admin";
    public string StaffRole { get; set; } = "Staff";
    public string AdminRoleLabel { get; set; } = "Quản trị viên";
    public string StaffRoleLabel { get; set; } = "Nhân viên";
    
    // Helper properties for filters
    public bool IsFilterAll => string.IsNullOrEmpty(RoleFilter);
    public bool IsFilterAdmin => RoleFilter == AdminRole;
    public bool IsFilterStaff => RoleFilter == StaffRole;
    
    public bool IsAccountFilterAll => string.IsNullOrEmpty(AccountRoleFilter);
    public bool IsAccountFilterAdmin => AccountRoleFilter == AdminRole;
    public bool IsAccountFilterStaff => AccountRoleFilter == StaffRole;
}

public class SystemAccountFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    [StringLength(120, ErrorMessage = "Họ tên không được quá 120 ký tự.")]
    public string CustomerName { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [StringLength(20, ErrorMessage = "Số điện thoại không được quá 20 ký tự.")]
    public string Phone { get; set; } = "";

    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [StringLength(120, ErrorMessage = "Email không được quá 120 ký tự.")]
    public string? Email { get; set; }

    [StringLength(200, ErrorMessage = "Địa chỉ không được quá 200 ký tự.")]
    public string? Address { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn vai trò.")]
    public string Role { get; set; } = "Staff";

    [StringLength(100, ErrorMessage = "Mật khẩu không được quá 100 ký tự.")]
    public string? Password { get; set; }
}

public class SystemAccountRowViewModel
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string Role { get; set; } = "";
    public DateTime CreatedTime { get; set; }
    public bool IsCurrentUser { get; set; }
    
    // Helper properties for view
    public bool IsAdminRole => Role == "Admin";
    public bool IsStaffRole => Role == "Staff";
}

public class SystemRoleAdminRowViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string? Description { get; set; }
    public bool IsSystem { get; set; }
    public int UserCount { get; set; }
}

public class SystemPermissionRowViewModel
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string? Description { get; set; }
    public string? Module { get; set; }
    public bool IsSelected { get; set; }
    public string UserMode { get; set; } = "Inherit";
    public DateTime? ExpiresAt { get; set; }
    public bool IsApplicationPermission { get; set; }
}

public class SystemUserOptionViewModel
{
    public int Id { get; set; }
    public string Label { get; set; } = "";
    public string Role { get; set; } = "";
}

public class SystemRoleOptionViewModel
{
    public string Name { get; set; } = "";
    public string DisplayName { get; set; } = "";
}

public class SystemRoleFormViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string? Description { get; set; }
}

public class SystemPermissionFormViewModel
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string? Description { get; set; }
    public string? Module { get; set; }
}

public class SystemUserPermissionInputViewModel
{
    public int PermissionId { get; set; }
    public string Mode { get; set; } = "Inherit";
    public DateTime? ExpiresAt { get; set; }
}
