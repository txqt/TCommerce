using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.Options;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Crmf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TCommerce.Core.Models.Common;
using TCommerce.Core.Models.Momo;
using TCommerce.Core.Models.Options;
using TCommerce.Core.Models.Orders;

namespace TCommerce.Services.MomoServices
{
    public interface IMomoService
    {
        Task<dynamic> CreatePaymentAsync(Order order);
        MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
    }
    public class MomoService : IMomoService
    {
        private readonly IOptions<MomoOptions> _options;

        public MomoService(IOptions<MomoOptions> options)
        {
            _options = options;
        }

        public async Task<dynamic> CreatePaymentAsync(Order order)
        {
            var amount = order.OrderTotal.ToString("0", CultureInfo.InvariantCulture);
            var orderInfo = "Ma khach hang: " + order.UserId + " Thanh toan don hang:" + order.Id;

            var rawData =
                $"partnerCode={_options.Value.PartnerCode}" +
                $"&accessKey={_options.Value.AccessKey}" +
                $"&requestId={order.Id.ToString()}" +
                $"&amount={amount}" +
                $"&orderId={order.Id.ToString()}" +
                $"&orderInfo={orderInfo}" +
                $"&returnUrl={_options.Value.ReturnUrl}" +
                $"&notifyUrl={_options.Value.NotifyUrl}" +
                $"&notifyUrl={_options.Value.NotifyUrl}" +
                $"&partnerCode={_options.Value.PartnerCode}" +
                $"&paymentCode={_options.Value.NotifyUrl}" +
                $"&extraData=";

            var signature = ComputeHmacSha256(rawData, _options.Value.SecretKey);

            var requestData = new
            {
                accessKey = _options.Value.AccessKey,
                partnerCode = _options.Value.PartnerCode,
                requestType = _options.Value.RequestType,
                notifyUrl = _options.Value.NotifyUrl,
                returnUrl = _options.Value.ReturnUrl,
                orderId = order.Id.ToString(),
                amount,
                orderInfo,
                requestId = order.Id.ToString(),
                extraData = "",
                signature
            };

            var jsonData = JsonConvert.SerializeObject(requestData);

            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, _options.Value.MomoApiUrl)
            {
                Content = new StringContent(jsonData, Encoding.UTF8, "application/json") // Thiết lập Content-Type ở đây
            };

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"HTTP request failed with status code: {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject(responseContent);
        }


        public MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection)
        {
            var amount = collection.First(s => s.Key == "amount").Value;
            var orderInfo = collection.First(s => s.Key == "orderInfo").Value;
            var orderId = collection.First(s => s.Key == "orderId").Value;
            return new MomoExecuteResponseModel()
            {
                Amount = amount,
                OrderId = orderId,
                OrderInfo = orderInfo
            };
        }

        private string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            byte[] hashBytes;

            using (var hmac = new HMACSHA256(keyBytes))
            {
                hashBytes = hmac.ComputeHash(messageBytes);
            }

            var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return hashString;
        }
    }
}
