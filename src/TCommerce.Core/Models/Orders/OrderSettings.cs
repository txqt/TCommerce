using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCommerce.Core.Interface;

namespace TCommerce.Core.Models.Orders
{
    public class OrderSettings : ISettings
    {
        public bool AutoUpdateOrderTotalsOnEditingOrder { get; set; }
    }
}
