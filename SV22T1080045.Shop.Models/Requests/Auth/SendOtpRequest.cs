using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SV22T1080045.Shop.Abstractions;

namespace SV22T1080045.Shop.Models.Requests.Auth
{
    public class SendOtpRequest
    {
        [Required]
        [RegularExpression(PhoneNumberHelper.VietnameseMobilePattern)]
        public string Phone { get; set; } = "";
    }
}
