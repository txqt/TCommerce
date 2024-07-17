using Microsoft.AspNetCore.Hosting;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Core.Models.Options;
using TCommerce.Core.Models.Orders;
using TCommerce.Core.Models.Payments;
using TCommerce.Core.Models.Response;
using TCommerce.Core.Models.SendMail;
using TCommerce.Core.Models.Users;
using TCommerce.Services.PriceCalulationServices;
using TCommerce.Services.ProductServices;
using TCommerce.Services.VNPayServices;

namespace TCommerce.Services.PaymentServices
{
    public class PaymentService : IPaymentService
    {
        private readonly List<PaymentMethod> _paymentMethods;
        private readonly IVNPayService _vnpayService;
        private readonly IOptions<UrlOptions> _urlOptions;
        public PaymentService(IVNPayService vnpayService, IOptions<UrlOptions> urlOptions)
        {
            _paymentMethods = new List<PaymentMethod>
            {
                new CoDPaymentMethod
                {
                    Selected = true,
                },
                new VNPayPaymentMethod()
            };
            _vnpayService = vnpayService;
            _urlOptions = urlOptions;
        }

        public List<PaymentMethod> GetAllPaymentMethods()
        {
            return _paymentMethods;
        }

        public PaymentMethod GetPaymentMethodBySystemName(string paymentSystemName)
        {
            return GetAllPaymentMethods().Where(x => x.PaymentMethodSystemName == paymentSystemName).FirstOrDefault();
        }

        public async Task<ServiceResponse<string>> ProcessPaymentAsync(Order order, PaymentMethod paymentMethod)
        {
            var result = new ServiceResponse<string>();
            switch (paymentMethod)
            {
                case CoDPaymentMethod codPayment:
                    result = new ServiceSuccessResponse<string>();
                    break;
                case VNPayPaymentMethod vNPayPaymentMethod:
                    string paymentUrl = _vnpayService.CreatePaymentUrl(order, _urlOptions.Value.ClientUrl);
                    result.Data = paymentUrl;
                    result.Success = true;
                    break;
                default:
                    // Handle other payment methods or throw an exception if the method is not supported
                    throw new NotSupportedException($"Payment method {paymentMethod.PaymentMethodSystemName} is not supported.");
            }

            return result;
        }
    }
}
