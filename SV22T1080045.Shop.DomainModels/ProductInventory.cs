using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SV22T1080045.Shop.DomainModels
{
    [Table("ProductInventories")]
    public class ProductInventory
    {
        [Key]
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public int LowStockThreshold { get; set; } = 5;

        public DateTime UpdatedTime { get; set; } = DateTime.Now;

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
    }
}
