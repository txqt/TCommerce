using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using NuGet.Protocol.Plugins;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using T.Library.Model;
using T.Library.Model.JwtToken;
using T.Library.Model.Response;
using T.Web.Areas.Services.AccountService;
using T.Web.Controllers;
using T.Web.Extensions;

namespace T.Web.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("/account/[action]")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IOptions<JwtOptions> _jwtOptions;

        public AccountController(IAccountService accountService, IOptions<JwtOptions> jwtOptions)
        {
            _accountService = accountService;
            _jwtOptions = jwtOptions;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel, string? returnUrl)
        {
            var loginResponse = await _accountService.Login(loginViewModel);
            var userPrincipal = this.ValidateToken(loginResponse.Data.AccessToken);
            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                IsPersistent = loginViewModel.RememberMe
            };
            // Lưu trữ giá trị vào session
            HttpContext.Session.SetString("jwt", loginResponse.Data.AccessToken);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, authProperties);

            if (returnUrl != null)
                return RedirectToLocal(returnUrl);
            return Redirect("/");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
        private ClaimsPrincipal ValidateToken(string jwtToken)
        {
            IdentityModelEventSource.ShowPII = true;

            SecurityToken validatedToken;
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

            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, tokenValidationParameters, out validatedToken);

            return principal;
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}
