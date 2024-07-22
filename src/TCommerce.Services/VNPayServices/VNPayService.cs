using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Orders;
using TCommerce.Core.Models.Response;
using TCommerce.Core.Utilities;

namespace TCommerce.Services.VNPayServices
{
    public class VNPayService : IVNPayService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;

        public VNPayService(IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            _configuration = configuration;
            _contextAccessor = contextAccessor;
        }

        public string CreatePaymentUrl(Order order, string baseUrl)
        {
            var vnPayConfig = _configuration.GetSection("VNPay");
            //string vnp_TmnCode = vnPayConfig["vnp_TmnCode"];
            string vnp_TmnCode = "***REMOVED***";
            //string vnp_HashSecret = vnPayConfig["vnp_HashSecret"];
            string vnp_HashSecret = "***REMOVED***";
            string vnp_Url = vnPayConfig["vnp_Url"];
            string vnp_ReturnUrl = vnPayConfig["vnp_ReturnUrl"];

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", Convert.ToInt32(decimal.Parse(order.OrderTotal) * 100).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            vnpay.AddRequestData("vnp_BankCode", "");
            vnpay.AddRequestData("vnp_CreateDate", order.CreatedOnUtc.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", AppUtilities.GetIpAddress(_contextAccessor));
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + order.OrderGuid);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_ReturnUrl);
            vnpay.AddRequestData("vnp_TxnRef", order.OrderGuid.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            //Billing

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return paymentUrl;
        }

        public async Task<ServiceResponse<string>> ProcessPaymentCallbackAsync(IQueryCollection queryParameters)
        {
            var vnPayConfig = _configuration.GetSection("VNPay");
            //string vnp_HashSecret = vnPayConfig["vnp_HashSecret"];
            string vnp_HashSecret = "***REMOVED***";

            var vnpay = new VnPayLibrary();
            foreach (var key in queryParameters.Keys)
            {
                if (key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, queryParameters[key]);
                }
            }

            bool isValidSignature = vnpay.ValidateSignature(vnpay.GetResponseData("vnp_SecureHash"), vnp_HashSecret);
            if (!isValidSignature)
            {
                return new ServiceErrorResponse<string> { Message = "Invalid signature" };
            }

            string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            if (vnp_ResponseCode == "00")
            {
                return new ServiceSuccessResponse<string>();
            }
            else
            {
                return new ServiceErrorResponse<string> { Message = "Payment failed" };
            }
        }
    }
}
