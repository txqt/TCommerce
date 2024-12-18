﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCommerce.Core.Models.Payments
{
    public class VNPayPaymentMethod : PaymentMethod
    {
        public VNPayPaymentMethod()
        {
            PaymentMethodSystemName = "Payment.VNPay";
            Name = "VNPay (không hỗ trợ test nữa)";
            Description = "Pay with VNPay";
            Fee = "0";
        }
    }
}
