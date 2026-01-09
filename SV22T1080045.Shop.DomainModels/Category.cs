using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV22T1080045.Shop.DomainModels
{
    [Table("Categories")]
    public class Category : _BaseEntity
    {
        public string CategoryName { get; set; } = "";
        public string? Description { get; set; }
        public string? ImageUrl { get; set; } 
    }
}
