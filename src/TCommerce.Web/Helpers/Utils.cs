﻿using System.Security.Cryptography;
using System.Text;

namespace TCommerce.Web.Helpers
{
    public class Utils
    {
        public static String HmacSHA512(string key, String inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }
        public static string GetIpAddress(IHttpContextAccessor httpContextAccessor)
        {
            var remoteIpAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress;
            return remoteIpAddress?.ToString();
        }
    }
}
