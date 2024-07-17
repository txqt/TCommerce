using Microsoft.AspNetCore.Mvc;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Payments;
using TCommerce.Core.Utilities;
using TCommerce.Services.VNPayServices;
using TCommerce.Web.Attribute;

namespace TCommerce.Web.Controllers
{
    [CheckPermission]
    public class PaymentController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IVNPayService _VNPayService;
        private readonly IOrderService _orderService;

        public PaymentController(IConfiguration configuration, IVNPayService VNPayService, IOrderService orderService)
        {
            _configuration = configuration;
            _VNPayService = VNPayService;
            _orderService = orderService;
        }

        [HttpGet]
        [Route("payment/vnpaycallback")]
        public async Task<IActionResult> VNPayCallback()
        {
            var queryParameters = Request.Query;

            var paymentResult = await _VNPayService.ProcessPaymentCallbackAsync(queryParameters);

            if (paymentResult.Success)
            {
                string orderId = queryParameters["vnp_TxnRef"].ToString();
                var order = await _orderService.GetOrderByGuidAsync(Guid.Parse(orderId));

                if (order != null)
                {
                    order.PaymentStatus = PaymentStatus.Paid; 
                    await _orderService.UpdateOrderAsync(order);

                    return Content("Payment successfully processed. Order ID: " + orderId);
                }
                else
                {
                    return Content("Order not found.");
                }
            }
            else
            {
                return Content(paymentResult.Message);
            }
        }
    }
}
