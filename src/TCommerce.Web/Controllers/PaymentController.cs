using Microsoft.AspNetCore.Mvc;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Payments;
using TCommerce.Core.Utilities;
using TCommerce.Services.MomoServices;
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
        private readonly IMomoService _momoService;

        public PaymentController(IConfiguration configuration, IVNPayService VNPayService, IOrderService orderService, IMomoService momoService)
        {
            _configuration = configuration;
            _VNPayService = VNPayService;
            _orderService = orderService;
            _momoService = momoService;
        }

        [HttpGet]
        [Route("payment/vnpaycallback")]
        public async Task<IActionResult> VNPayCallBack()
        {
            var queryParameters = Request.Query;

            var paymentResult = await _VNPayService.ProcessPaymentCallBackAsync(queryParameters);

            if (paymentResult.Success)
            {
                string orderId = queryParameters["vnp_TxnRef"].ToString();
                var order = await _orderService.GetOrderByGuidAsync(Guid.Parse(orderId));

                if (order != null)
                {
                    order.PaymentStatus = PaymentStatus.Paid;
                    await _orderService.UpdateOrderAsync(order);

                    return View("Thankyou", $"#{orderId}");
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

        [HttpGet]
        [Route("payment/momocallback")]
        public async Task<IActionResult> MomoCallBack()
        {
            var response = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);

            return View("Thankyou", $"#{response.OrderId} {response.Amount} {response.OrderInfo}");
        }
    }
}
