using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV22T1080045.Shop.DomainModels
{
    [Table("Units")]
    public class Unit : _BaseEntity
    {
        public string UnitName { get; set; } = ""; 
    }
}
