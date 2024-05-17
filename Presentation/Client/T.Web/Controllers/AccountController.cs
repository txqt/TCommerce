using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using T.Library.Model;
using T.Library.Model.Account;
using T.Library.Model.Common;
using T.Library.Model.ViewsModel;
using T.Web.Models;
using T.Web.Services.PrepareModelServices;
using T.Web.Services.UserRegistrationServices;
using T.Web.Services.UserService;

namespace T.Web.Controllers
{
    //[Route("account/[action]")]
    public class AccountController : BaseController
    {
        private readonly IUserRegistrationService _accountService;
        private readonly IUserService _userService;
        private readonly IOptions<Library.Model.JwtToken.AuthorizationOptionsConfig> _jwtOptions;
        private readonly IAccountModelService _accountModelService;
        private readonly IMapper _mapper;

        public AccountController(IUserRegistrationService accountService, IOptions<Library.Model.JwtToken.AuthorizationOptionsConfig> jwtOptions, IUserService userService, IAccountModelService accountModelService, IMapper mapper)
        {
            _accountService = accountService;
            _jwtOptions = jwtOptions;
            _userService = userService;
            _accountModelService = accountModelService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var loginVM = new AccessTokenRequestModel()
            {
                RememberMe = true
            };
            ViewBag.ReturnUrl = returnUrl;
            return View(loginVM);
        }

        [HttpPost]
        public async Task<IActionResult> Login(AccessTokenRequestModel loginViewModel, string returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(loginViewModel);

            var loginResponse = await _accountService.Login(loginViewModel);

            if (!loginResponse.Success)
            {
                ModelState.AddModelError(string.Empty, loginResponse.Message);
                return View(loginViewModel);
            }

            var userPrincipal = ValidateToken(loginResponse.Data.AccessToken);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(_jwtOptions.Value.RefreshTokenExpirationInSenconds),
                IsPersistent = loginViewModel.RememberMe
            };

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, // Chỉ cho phép server truy cập cookie, tránh XSS
                Secure = true, // Chỉ gửi cookie qua HTTPS, tránh sniffing
                SameSite = SameSiteMode.Strict, // Đặt chế độ SameSite cho cookie
                Expires = DateTimeOffset.UtcNow.AddSeconds(_jwtOptions.Value.AccessTokenExpirationInSenconds) // Đặt thời gian hết hạn cho cookie
            };
            var cookieRefreshTokenOptions = new CookieOptions
            {
                HttpOnly = true, // Chỉ cho phép server truy cập cookie, tránh XSS
                Secure = true, // Chỉ gửi cookie qua HTTPS, tránh sniffing
                SameSite = SameSiteMode.Strict, // Đặt chế độ SameSite cho cookie
                Expires = DateTimeOffset.UtcNow.AddSeconds(_jwtOptions.Value.RefreshTokenExpirationInSenconds) // Đặt thời gian hết hạn cho cookie
            };

            Response.Cookies.Append("jwt", loginResponse.Data.AccessToken, cookieOptions);
            Response.Cookies.Append("refreshToken", loginResponse.Data.RefreshToken, cookieRefreshTokenOptions);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, authProperties);


            if (returnUrl != null)
                return RedirectToLocal(returnUrl);

            return Redirect("/");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Xóa cookie
            Response.Cookies.Delete("jwt");
            Response.Cookies.Delete("refreshToken");

            await _accountService.Logout();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult Register()
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
            var result = await _userService.Register(registerRequest);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(registerRequest);
            }


            return LocalRedirect(Url.Action(nameof(RegisterConfirmation)));
        }

        [HttpGet]
        [Route("/account/forgot-password")]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [Route("/account/forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var result = await _accountService.SendResetPasswordEmail(email);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View();
            }


            return LocalRedirect(Url.Action(nameof(ForgotPasswordConfirmation)));
        }

        [HttpGet]
        [Route("/account/reset-password")]
        public IActionResult ResetPassword()
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

        [HttpGet]
        public async Task<IActionResult> Info()
        {
            var user = await _userService.GetCurrentUser();
            var model = new AccountInfoModel()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.Dob,
                Email = user.Email
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Info(AccountInfoModel model)
        {
            if (!ModelState.IsValid)
            {
                RedirectToAction(nameof(Info));
            }

            var result = await _userService.UpdateUserAccountInfo(model);

            SetStatusMessage(result.Success ? "Success" : "Failed");

            return RedirectToAction(nameof(Info));
        }

        [HttpGet]
        public async Task<IActionResult> Addresses()
        {
            return View(await _userService.GetOwnAddressesAsync());
        }

        [HttpGet]
        public async Task<IActionResult> CreateAddress()
        {
            var model = await _accountModelService.PrepareDeliveryAddressModel(null, new DeliveryAddressModel());
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAddress(DeliveryAddressModel model)
        {
            if (!ModelState.IsValid)
            {
                model = await _accountModelService.PrepareDeliveryAddressModel(null, model);
                return View(model);
            }

            var address = _mapper.Map<DeliveryAddress>(model);

            address.CreatedOnUtc = DateTime.Now;

            var result = await _userService.CreateUserAddressAsync(address);

            SetStatusMessage(result.Success ? "Success" : "Failed");

            return RedirectToAction(nameof(Addresses));
        }

        [HttpGet]
        public IActionResult UpdateAddress()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UpdateAddress(DeliveryAddress address)
        {
            return View();
        }
    }
}
