using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV22T1080045.Shop.Models.Requests.Auth
{
    public class SendOtpRequest
    {
        [Required]
        [RegularExpression(@"^(0|\+84)[3-9]\d{8}$")]
        public string Phone { get; set; } = "";
    }
}
