namespace SV22T1080045.Shop.Abstractions.Models.System;

public class SystemAccountData
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string Role { get; set; } = "";
    public DateTime CreatedTime { get; set; }
    public bool IsDeleted { get; set; }
}

public class SystemAccountSaveResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public int AccountId { get; set; }
}

public class SystemRoleUpdateResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
}
