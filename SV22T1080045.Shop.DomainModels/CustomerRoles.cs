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
}
