namespace SV22T1080045.Shop.Models.ViewModels.Checkout
{
    public class QrPaymentViewModel
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = "";
        public string QrImageUrl { get; set; } = "";
        public string BankId { get; set; } = "";
        public string AccountNumber { get; set; } = "";
        public string AccountName { get; set; } = "";
        public decimal Amount { get; set; }
        public string TransferContent { get; set; } = "";
        public bool IsGuest { get; set; }
    }
}
