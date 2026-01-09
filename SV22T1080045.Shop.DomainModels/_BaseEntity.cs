using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV22T1080045.Shop.DomainModels
{
    public class _BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.Now;

        public bool IsDeleted { get; set; } = false; // Dùng để xóa mềm (ẩn đi thay vì xóa thật)
    }
}
