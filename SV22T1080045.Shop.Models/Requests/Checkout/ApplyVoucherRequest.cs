using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV22T1080045.Shop.Models.Requests.Checkout
{
    public record ApplyVoucherRequest(string Code, decimal OrderAmount);
}
