using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.Options;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
using System.Text.Json;
using System.Threading.Tasks;
using TCommerce.Core.Models.Common;
using TCommerce.Core.Models.Momo;
using TCommerce.Core.Models.Options;
using TCommerce.Core.Models.Orders;
using static System.Net.Mime.MediaTypeNames;

namespace TCommerce.Services.MomoServices
{
    public interface IMomoService
    {
        Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(Order order);
        MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
    }
    public class MomoService : IMomoService
    {
        private readonly IOptions<MomoOptions> _options;
        private readonly HttpClient _httpClient;

        public MomoService(IOptions<MomoOptions> options, HttpClient httpClient)
        {
            _options = options;
            _httpClient = httpClient;
        }

        public async Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(Order model)
        {
            CollectionLinkRequest request = new CollectionLinkRequest();
            request.OrderInfo = "(TCommerce) Thanh toan don hang #"+model.OrderGuid.ToString();
            request.PartnerCode = _options.Value.PartnerCode;
            request.RedirectUrl = "";
            request.IpnUrl = "https://webhook.site/b3088a6a-2d17-4f8d-a383-71389a6c600b";
            request.RedirectUrl = _options.Value.ReturnUrl;
            request.Amount = (long)model.OrderTotal;
            request.OrderId = model.OrderGuid.ToString();
            request.RequestId = model.OrderGuid.ToString();
            request.RequestType = _options.Value.RequestType;
            request.ExtraData = "";
            request.PartnerName = "MoMo Payment";
            request.StoreId = "Test Store";
            request.OrderGroupId = "";
            request.AutoCapture = true;
            request.Lang = "vi";

            var rawSignature = "accessKey=" + _options.Value.AccessKey + "&amount=" + request.Amount + "&extraData=" + request.ExtraData + "&ipnUrl=" + request.IpnUrl + "&orderId=" + request.OrderId + "&orderInfo=" + request.OrderInfo + "&partnerCode=" + request.PartnerCode + "&redirectUrl=" + request.RedirectUrl + "&requestId=" + request.RequestId + "&requestType=" + request.RequestType;
            request.Signature = ComputeHmacSha256(rawSignature, _options.Value.SecretKey);

            StringContent httpContent = new StringContent(System.Text.Json. JsonSerializer.Serialize(request, new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }), System.Text.Encoding.UTF8, "application/json");
            var quickPayResponse = await _httpClient.PostAsync("https://test-payment.momo.vn/v2/gateway/api/create", httpContent);
            var contents = quickPayResponse.Content.ReadAsStringAsync().Result;
            System.Console.WriteLine(contents + "");

            return JsonConvert.DeserializeObject<MomoCreatePaymentResponseModel>(contents);
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
            ASCIIEncoding encoding = new ASCIIEncoding();

            Byte[] textBytes = encoding.GetBytes(message);
            Byte[] keyBytes = encoding.GetBytes(secretKey);

            Byte[] hashBytes;

            using (HMACSHA256 hash = new HMACSHA256(keyBytes))
                hashBytes = hash.ComputeHash(textBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
