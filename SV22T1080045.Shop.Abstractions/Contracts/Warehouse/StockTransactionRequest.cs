namespace SV22T1080045.Shop.Abstractions.Contracts.Warehouse;

public class StockTransactionRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string TransactionType { get; set; } = "Import";
    public decimal UnitPrice { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Reason { get; set; }
    public DateTime TransactionDate { get; set; }
}

public class ReimportStockRequest
{
    public int OrderId { get; set; }
    public int OrderDetailId { get; set; }
    public int Quantity { get; set; }
    public string? Reason { get; set; }
    public DateTime TransactionDate { get; set; }
}

public class CustomerDebtRequest
{
    public int CustomerId { get; set; }
    public string? OrderCode { get; set; }
    public decimal DebtAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public string? PaymentReference { get; set; }
    public DateTime DueDate { get; set; }
    public string? Notes { get; set; }
}
