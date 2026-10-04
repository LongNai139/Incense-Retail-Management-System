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
    public List<SystemRoleDefinitionViewModel> RoleDefinitions { get; set; } = new();
    public int CurrentUserId { get; set; }
    
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
    public string CustomerName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string Role { get; set; } = "Staff";
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

public class SystemRoleDefinitionViewModel
{
    public string Role { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string Description { get; set; } = "";
    public List<string> Permissions { get; set; } = new();
    public int AccountCount { get; set; }
}
