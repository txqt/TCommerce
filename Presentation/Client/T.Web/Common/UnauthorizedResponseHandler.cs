using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NuGet.Protocol.Plugins;
using System.Net;
using System.Net.Http.Headers;
using T.Library.Model.JwtToken;
using T.Library.Model.RefreshToken;
using T.Library.Model.Response;

namespace T.Web.Common
{
    public class UnauthorizedResponseHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IOptions<JwtOptions> _jwtOptions;

        public UnauthorizedResponseHandler(IHttpContextAccessor httpContextAccessor, IHttpClientFactory clientFactory, IOptions<JwtOptions> jwtOptions)
        {
            _httpContextAccessor = httpContextAccessor;
            _clientFactory = clientFactory;
            _jwtOptions = jwtOptions;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var refreshToken = _httpContextAccessor.HttpContext.Request.Cookies["refreshToken"];
                var accessToken = _httpContextAccessor.HttpContext.Request.Cookies["jwt"];

                if (!string.IsNullOrEmpty(refreshToken) && !string.IsNullOrEmpty(accessToken))
                {
                    var client = _clientFactory.CreateClient();
                    RefreshTokenDto refreshTokenDto = new RefreshTokenDto()
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        ReturnUrl = ""
                    };
                    var refreshResponse = await client.PostAsJsonAsync("api/account/refresh", refreshTokenDto);

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
                                Expires = DateTimeOffset.UtcNow.AddHours(_jwtOptions.Value.AccessTokenExpirationInHours) // Đặt thời gian hết hạn cho cookie
                            };
                            var cookieRefreshTokenOptions = new CookieOptions
                            {
                                HttpOnly = true, // Chỉ cho phép server truy cập cookie, tránh XSS
                                Secure = true, // Chỉ gửi cookie qua HTTPS, tránh sniffing
                                SameSite = SameSiteMode.Strict, // Đặt chế độ SameSite cho cookie
                                Expires = DateTimeOffset.UtcNow.AddHours(_jwtOptions.Value.RefreshTokenExpirationInHours) // Đặt thời gian hết hạn cho cookie
                            };
                            _httpContextAccessor.HttpContext.Response.Cookies.Append("jwt", result.Data.AccessToken, cookieOptions);
                            _httpContextAccessor.HttpContext.Response.Cookies.Append("refreshToken", result.Data.RefreshToken, cookieOptions);

                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.Data.AccessToken);
                            response = await base.SendAsync(request, cancellationToken);
                        }
                    }
                    else
                    {
                        _httpContextAccessor.HttpContext.Response.Redirect("/Account/Login");
                    }
                }
            }

            return response;
        }
    }
}
