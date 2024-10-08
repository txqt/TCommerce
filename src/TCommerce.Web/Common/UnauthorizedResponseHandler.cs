﻿using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;
using TCommerce.Core.Models.JwtToken;
using TCommerce.Core.Models.RefreshToken;
using TCommerce.Core.Models.Response;

namespace TCommerce.Web.Common
{
    public class UnauthorizedResponseHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IOptions<AuthorizationOptionsConfig> _jwtOptions;
        //private readonly IAccountService _accountService;

        public UnauthorizedResponseHandler(IHttpContextAccessor httpContextAccessor, IHttpClientFactory clientFactory, IOptions<AuthorizationOptionsConfig> jwtOptions/*, IAccountService accountService*/)
        {
            _httpContextAccessor = httpContextAccessor;
            _clientFactory = clientFactory;
            _jwtOptions = jwtOptions;
            //_accountService = accountService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                var refreshToken = _httpContextAccessor.HttpContext.Request.Cookies["refreshToken"];

                if (!string.IsNullOrEmpty(refreshToken))
                {
                    var client = _clientFactory.CreateClient();
                    RefreshTokenRequestModel refreshTokenDto = new RefreshTokenRequestModel()
                    {
                        RefreshToken = refreshToken,
                        ReturnUrl = ""
                    };
                    var refreshResponse = await client.PostAsJsonAsync("api/token/refresh", refreshTokenDto);

                    if (refreshResponse.IsSuccessStatusCode)
                    {
                        var result = await refreshResponse.Content.ReadFromJsonAsync<ServiceResponse<AuthResponseDto>>();
                        if (result.Success)
                        {
                            var cookieOptions = new CookieOptions
                            {
                                HttpOnly = true, // Chỉ cho phép server truy cập cookie, tránh XSS
                                Secure = true, // Chỉ gửi cookie qua HTTPS, tránh sniffing
                                SameSite = SameSiteMode.Strict, // Đặt chế độ SameSite cho cookie
                                Expires = DateTimeOffset.UtcNow.AddSeconds(_jwtOptions.Value.AccessTokenExpirationInSenconds)
                            };
                            _httpContextAccessor.HttpContext.Response.Cookies.Append("jwt", result.Data.AccessToken, cookieOptions);

                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.Data.AccessToken);
                            response = await base.SendAsync(request, cancellationToken);
                        }
                    }
                    else
                    {
                        _httpContextAccessor.HttpContext.Response.Cookies.Delete("refreshToken");
                        _httpContextAccessor.HttpContext.Response.Redirect("/Account/Login");
                        //await _accountService.Logout();
                    }
                }
            }

            return response;
        }
    }
}
