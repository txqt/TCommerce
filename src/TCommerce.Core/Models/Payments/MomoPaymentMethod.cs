using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCommerce.Core.Models.Payments
{
    public class MomoPaymentMethod : PaymentMethod
    {
        public MomoPaymentMethod()
        {
            PaymentMethodSystemName = "Payment.Momo";
            Name = "Momo";
            Description = "Pay with momo";
            Fee = "0";
        }
    }
}
