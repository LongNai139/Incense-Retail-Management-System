namespace SV22T1080045.Shop.Models.ViewModels.Account;

public class BackofficeLoginBridgeViewModel
{
    public string BackofficeLoginUrl { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Password { get; set; } = "";
    public string ReturnUrl { get; set; } = "/Management";
}
