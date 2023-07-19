﻿
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;
using T.Library.Model.JwtToken;

namespace T.Web.CusomMiddleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptions<JwtOptions> _jwtOptions;

        public JwtMiddleware(RequestDelegate next, IHttpContextAccessor httpContextAccessor, IOptions<JwtOptions> jwtOptions)
        {
            _next = next;
            _httpContextAccessor = httpContextAccessor;
            _jwtOptions = jwtOptions;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var jwtToken = _httpContextAccessor.HttpContext.Session.GetString("jwtToken");

            if (!string.IsNullOrEmpty(jwtToken))
            {
                // Kiểm tra tính hợp lệ của JWT token
                if (IsJwtTokenValid(jwtToken))
                {
                    context.Request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken).ToString();
                }
                else
                {
                    // Xóa JWT token khỏi session
                    _httpContextAccessor.HttpContext.Session.Remove("jwtToken");
                }
            }

            await _next(context);
        }

        private bool IsJwtTokenValid(string jwtToken)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtOptions.Value.Issuer,
                ValidAudience = _jwtOptions.Value.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.AccessTokenKey)),
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(jwtToken, tokenValidationParameters, out securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }


            return true; // hoặc false nếu JWT token không hợp lệ
        }
    }



}
