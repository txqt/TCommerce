using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCommerce.Core.Models.Orders;
using TCommerce.Core.Models.Response;

namespace TCommerce.Core.Interface
{
    public interface IVNPayService
    {
        string CreatePaymentUrl(Order order, string baseUrl);
        Task<ServiceResponse<string>> ProcessPaymentCallbackAsync(IQueryCollection queryParameters);
    }
}
