namespace SV22T1080045.Shop.DomainModels.Warehouse
{
    /// <summary>
    /// Sổ công nợ chi tiết - Lưu lịch sử biến động công nợ khách hàng
    /// Mỗi giao dịch tăng/giảm công nợ đều phải tạo một bản ghi
    /// </summary>
    public class CustomerDebtTransaction
    {
        public int Id { get; set; }
        public int CustomerDebtId { get; set; }  // FK đến CustomerDebt
        public int? OrderId { get; set; }         // FK đến Orders (nullable)
        public string TransactionType { get; set; } = ""; // DEBIT (tăng nợ), CREDIT (giảm nợ)
        public decimal Amount { get; set; }
        public decimal BalanceAfter { get; set; } // Số dư sau giao dịch
        public string? Reference { get; set; }    // TransactionNo, payment reference
        public string? Description { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime CreatedTime { get; set; }
        public int? CreatedBy { get; set; }      // StaffId nếu có
        public bool IsDeleted { get; set; }

        // Navigation properties
        public virtual CustomerDebt CustomerDebt { get; set; } = null!;
        public virtual Order? Order { get; set; }
    }
}
