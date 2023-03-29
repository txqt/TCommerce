using App.Utilities;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using T.Library.Model;
using T.Library.Model.RefreshToken;
using T.Library.Model.Response;
using T.Library.Model.Users;
using T.WebApi.Database.ConfigurationDatabase;
using T.WebApi.Extensions;
using T.WebApi.Helpers.TokenHelpers;

namespace T.WebApi.Services.AccountServices
{
    public interface IAccountService
    {
        Task<LoginResponse<AuthResponseDto>> Login(LoginViewModel login, string? returnUrl);
    }
    public class AccountService : IAccountService
    {
        private readonly IConfiguration _configuration;
        private readonly SignInManager<User> _signInManager;
        private readonly IHttpContextAccessor _httpContext;
        //private readonly IEmailService _emailService;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly DatabaseContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ITokenService _tokenService;

        public AccountService(IConfiguration configuration,
                               UserManager<User> userManager,
                               SignInManager<User> signInManager,
                               //IEmailService emailService,
                               RoleManager<Role> roleManager,
                               DatabaseContext context,
                               IHttpContextAccessor httpContext,
                               IWebHostEnvironment env,
                               ITokenService tokenService)
        {
            //this._emailService = emailService;
            _configuration = configuration;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _env = env;
            this._tokenService = tokenService;
            _httpContext = httpContext;
            _context = context;
        }
        public async Task<LoginResponse<AuthResponseDto>> Login(LoginViewModel login, string? returnUrl)
        {
            var user = await _userManager.FindByNameAsync(login.UserNameOrEmail);
            if (user == null) return new LoginResponse<AuthResponseDto>() { Message = "Tài khoản không tồn tại", Success = false };

            var result = await _signInManager.PasswordSignInAsync(login.UserNameOrEmail, login.Password, login.RememberMe, lockoutOnFailure: true);
            
            // Tìm UserName theo Email, đăng nhập lại
            if ((!result.Succeeded) && AppUtilities.IsValidEmail(login.UserNameOrEmail))
            {
                user = await _userManager.FindByEmailAsync(login.UserNameOrEmail);
                if (user != null)
                {
                    result = await _signInManager.PasswordSignInAsync(user.UserName, login.Password, login.RememberMe, lockoutOnFailure: true);
                }
            }

            

            if (result.IsNotAllowed) 
                return new LoginResponse<AuthResponseDto>() { Message = "Tài khoản không được cấp quyền vào trang này", Success = false };

            if (result.IsLockedOut)
            {
                //AppExtensions.GetDateTimeNow();
                var timeLockOutEnd = DateTimeOffset.Now - user.LockoutEnd;
                return new LoginResponse<AuthResponseDto>() { Message = $"Thử lại sau {timeLockOutEnd.Value}", Success = false };
            }

            if (!result.Succeeded)
                return new LoginResponse<AuthResponseDto>() { Message = "Tài khoản hoặc mật khẩu không đúng", Success = false };


            var roles = await _userManager.GetRolesAsync(user);


            var signingCredentials = _tokenService.GetSigningCredentials();
            var claims = await _tokenService.GetClaims(user);
            var tokenOptions = _tokenService.GenerateTokenOptions(signingCredentials, claims);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            var refreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = AppExtensions.GetDateTimeNow().AddDays(7);
            await _userManager.UpdateAsync(user);
            var response = new AuthResponseDto()
            {
                Token = token,
                RefreshToken = refreshToken,
                ReturnUrl = returnUrl
            };

            return new LoginResponse<AuthResponseDto>() { Success = true, Data = response, RoleNames = roles.ToList() };
        }
    }
}
