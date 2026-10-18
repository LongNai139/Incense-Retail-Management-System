namespace SV22T1080045.Shop.Abstractions.Models.Warehouse;

public class StockDashboardData
{
    public int TransactionCount { get; set; }
    public int ImportCount { get; set; }
    public int ExportCount { get; set; }
}

public class StockTopExportData
{
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public int Quantity { get; set; }
}
