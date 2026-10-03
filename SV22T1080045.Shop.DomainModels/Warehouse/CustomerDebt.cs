namespace SV22T1080045.Shop.DomainModels.Warehouse
{
    public class CustomerDebt
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; } // Customer name from join
        
        public int? OrderId { get; set; }
        public string? OrderCode { get; set; } // Store order code instead of reference
        public decimal OrderAmount { get; set; } // Store order amount separately
        
        public decimal DebtAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount => DebtAmount - PaidAmount;
        
        public string? PaymentReference { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? PaidDate { get; set; }
        
        public string Status { get; set; } = "Pending"; // Pending, PartiallyPaid, Paid, Overdue
        public string? Notes { get; set; }
        
        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        
        public bool IsDeleted { get; set; }
        
        public bool IsOverdue => Status == "Pending" && DateTime.Today > DueDate;
    }
}