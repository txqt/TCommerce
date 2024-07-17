using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCommerce.Core.Models.Orders;
using TCommerce.Core.Models.Payments;
using TCommerce.Core.Models.Response;

namespace TCommerce.Core.Interface
{
    public interface IPaymentService
    {
        public List<PaymentMethod> GetAllPaymentMethods();
        public PaymentMethod GetPaymentMethodBySystemName(string paymentSystemName);
        public Task<ServiceResponse<string>> ProcessPaymentAsync(Order oder, PaymentMethod paymentMethod);
    }
}
