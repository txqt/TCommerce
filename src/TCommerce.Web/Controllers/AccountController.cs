using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Accounts;
using TCommerce.Core.Models.Common;
using TCommerce.Core.Models.JwtToken;
using TCommerce.Core.Models.Users;
using TCommerce.Core.Models.ViewsModel;
using TCommerce.Web.Models;
using TCommerce.Web.PrepareModelServices;

namespace TCommerce.Web.Controllers
{
    //[Route("account/[action]")]
    public class AccountController : BaseController
    {
        private readonly IUserRegistrationService _accountService;
        private readonly IUserService _userService;
        private readonly IOptions<AuthorizationOptionsConfig> _jwtOptions;
        private readonly IMapper _mapper;
        private readonly IAddressService _addressService;
        private readonly IAccountModelService _accountModelService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserRegistrationService _userRegistrationService;

        public AccountController(IUserRegistrationService accountService, IOptions<AuthorizationOptionsConfig> jwtOptions, IUserService userService, IMapper mapper, IAddressService addressService, IAccountModelService accountModelService, UserManager<User> userManager, SignInManager<User> signInManager, IUserRegistrationService userRegistrationService)
        {
            _accountService = accountService;
            _jwtOptions = jwtOptions;
            _userService = userService;
            _mapper = mapper;
            _addressService = addressService;
            _accountModelService = accountModelService;
            _userManager = userManager;
            _signInManager = signInManager;
            _userRegistrationService = userRegistrationService;
        }

        //[HttpGet]
        //public async Task<IActionResult> Login(string returnUrl)
        //{
        //    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //    var loginVM = new AccessTokenRequestModel()
        //    {
        //        RememberMe = true
        //    };
        //    ViewBag.ReturnUrl = returnUrl;
        //    return View(loginVM);
        //}

        [HttpPost]
        public async Task<IActionResult> Login(AccessTokenRequestModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.UserNameOrEmail)
                           ?? await _userManager.FindByNameAsync(model.UserNameOrEmail);

                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, false, lockoutOnFailure: true);

                    if (result.Succeeded)
                    {
                        // Reset access failed count upon successful login
                        await _userManager.ResetAccessFailedCountAsync(user);

                        // Redirect to the returnUrl if provided, or to Home/Index
                        return RedirectToAction("Index", "Home");
                    }
                    else if (result.IsLockedOut)
                    {
                        ModelState.AddModelError(string.Empty, "Your account has been locked out due to multiple failed login attempts.");
                    }
                    else
                    {
                        await _userManager.AccessFailedAsync(user);
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            // If we reach here, something went wrong, return the model with errors
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Xóa cookie
            Response.Cookies.Delete("jwt");
            Response.Cookies.Delete("refreshToken");
            await _signInManager.SignOutAsync();
            await _accountService.Logout();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("SignInOrSignUp", "Account");
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
            var model = _mapper.Map<UserModel>(registerRequest);
            var result = await _userService.CreateUserAsync(model);

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
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed
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

            //get the data again so you don't miss any fields
            var user = await _userService.GetCurrentUser();

            _mapper.Map(model, user);

            var result = await _userService.UpdateUserAsync(user);

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
            var model = await _accountModelService.PrepareAddressModel(null, new AddressModel());
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAddress(AddressModel model)
        {
            if (!ModelState.IsValid)
            {
                model = await _accountModelService.PrepareAddressModel(null, model);
                return View(model);
            }

            var address = _mapper.Map<Address>(model);

            address.CreatedOnUtc = DateTime.Now;

            var result = await _userService.CreateUserAddressAsync(address);

            SetStatusMessage(result.Success ? "Success" : "Failed");

            return RedirectToAction(nameof(Addresses));
        }

        [HttpGet]
        public async Task<IActionResult> UpdateAddress(int id)
        {
            var currentAddress = await _addressService.GetAddressByIdAsync(id);

            var model = await _accountModelService.PrepareAddressModel(currentAddress, null);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAddress(AddressModel model)
        {
            if (!ModelState.IsValid)
            {
                var currentAddress = await _addressService.GetAddressByIdAsync(model.Id);

                model = await _accountModelService.PrepareAddressModel(currentAddress, model);

                return View(model);
            }

            var address = _mapper.Map<Address>(model);

            address.CreatedOnUtc = DateTime.Now;

            var result = await _userService.UpdateUserAddressAsync(address);

            SetStatusMessage(result.Success ? "Success" : "Failed");

            return RedirectToAction(nameof(Addresses));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAddressAsync(int id)
        {

            var result = await _userService.DeleteUserAddressAsync(id);

            SetStatusMessage(result.Success ? "Success" : "Failed");

            return RedirectToAction(nameof(Addresses));
        }

        [HttpGet]
        public async Task<IActionResult> SignInOrSignUp(string returnUrl)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var accessTokenRequest = new AccessTokenRequestModel()
            {
                RememberMe = true
            };

            ViewBag.ReturnUrl = returnUrl;

            var model = new SignInOrSignUpModel
            {
                AccessTokenRequest = accessTokenRequest,
                RegisterRequest = new RegisterRequest()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendConfirmationEmail()
        {
            var user = await _userManager.GetUserAsync(User);

            var result = await _userRegistrationService.SendConfirmationEmail(user);

            if (result.Success)
            {
                return Ok(result.Message);
            }

            return BadRequest(result.Message);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var result = await _userRegistrationService.ConfirmEmail(userId, token);

            return View(result.Success ? "ConfirmEmailSuccessfully" : "ConfirmEmailError");
        }
    }
}
