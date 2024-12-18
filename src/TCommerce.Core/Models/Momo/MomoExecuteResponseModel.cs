using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCommerce.Core.Models.Momo
{
    public class MomoExecuteResponseModel
    {
        public string Amount { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string OrderInfo { get; set; } = string.Empty;
    }
}
