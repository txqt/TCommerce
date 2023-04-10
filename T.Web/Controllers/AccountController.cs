using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
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
using T.Library.Model.Account;
using T.Library.Model.JwtToken;
using T.Library.Model.Response;
using T.Web.Attribute;
using T.Web.Extensions;
using T.Web.Services.AccountService;

namespace T.Web.Controllers
{
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
            if (!ModelState.IsValid)
                return View(loginViewModel);

            var loginResponse = await _accountService.Login(loginViewModel);

            if (!loginResponse.Success)
            {
                ModelState.AddModelError(string.Empty, loginResponse.Message);
                return View(loginViewModel);
            }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var userPrincipal = ValidateToken(loginResponse.Data.AccessToken);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
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
            await _accountService.Logout();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            if (!ModelState.IsValid)
            {
                return View(registerRequest);
            }
            var result = await _accountService.Register(registerRequest);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(registerRequest);
            }


            return LocalRedirect(Url.Action(nameof(RegisterConfirmation)));
        }

        [HttpGet]
        [Route("/account/forgot-password")]
        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [Route("/account/forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgotPasswordViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(forgotPasswordViewModel);
            }
            var result = await _accountService.ForgotPassword(forgotPasswordViewModel);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(forgotPasswordViewModel);
            }


            return LocalRedirect(Url.Action(nameof(ForgotPasswordConfirmation)));
        }

        [HttpGet]
        [Route("/account/reset-password")]
        public async Task<IActionResult> ResetPassword()
        {
            return View();
        }
        [HttpPost]
        [Route("/account/reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest resetPasswordRequest)
        {
            if (!ModelState.IsValid)
            {
                return View(resetPasswordRequest);
            }
            var result = await _accountService.ResetPassword(resetPasswordRequest);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(resetPasswordRequest);
            }


            return LocalRedirect(Url.Action(nameof(ResetPasswordConfirmation)));
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
        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}
