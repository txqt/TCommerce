using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCommerce.Core.Models.Payments
{
    public class CoDPaymentMethod : PaymentMethod
    {
        /// <summary>
        /// Additional properties or methods specific to CoD can be added here.
        /// </summary>
        public CoDPaymentMethod()
        {
            // Set the default system name for CoD
            PaymentMethodSystemName = "Payment.CashOnDelivery";
            Name = "Cash on Delivery";
            Description = "Pay with cash upon delivery";
            Fee = "0";
        }

        // You can add any other specific properties or methods for CoD here
    }
}
