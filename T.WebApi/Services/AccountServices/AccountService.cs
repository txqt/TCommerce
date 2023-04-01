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
        Task<User> FindUserByName(string userName);
        Task<ServiceResponse<AuthResponseDto>> RefreshToken(RefreshTokenDto tokenDto);
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

        public async Task<User> FindUserByName(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
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

            //Create token
            
            var accessToken = await _tokenService.GenerateAccessToken(user);

            //Create refresh token
            var refreshToken = await _tokenService.GenerateRefreshToken();

            //Save refresh token
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = AppExtensions.GetDateTimeNow().AddDays(7);
            await _userManager.UpdateAsync(user);

            var response = new AuthResponseDto()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ReturnUrl = returnUrl
            };

            return new LoginResponse<AuthResponseDto>() { Success = true, Data = response, RoleNames = roles.ToList() };
        }

        public async Task<ServiceResponse<AuthResponseDto>> RefreshToken(RefreshTokenDto tokenDto)
        {
            try
            {
                if (tokenDto is null)
                {
                    return new ServiceErrorResponse<AuthResponseDto> { Success = false, Message = "Invalid client request" };
                }

                var principal = _tokenService.GetPrincipalFromExpiredToken(tokenDto.Token);
                var username = principal.Identity.Name;
                var user = await _userManager.FindByNameAsync(username);

                if (user == null || user.RefreshToken != tokenDto.RefreshToken || user.RefreshTokenExpiryTime <= AppExtensions.GetDateTimeNow())
                    return new ServiceErrorResponse<AuthResponseDto> { Success = false, Message = "Invalid client request" };

                var accessToken = await _tokenService.GenerateAccessToken(user);
                user.RefreshToken = await _tokenService.GenerateRefreshToken();
                await _userManager.UpdateAsync(user);

                var data = new AuthResponseDto()
                {
                    AccessToken = accessToken,
                    RefreshToken = user.RefreshToken,
                    ReturnUrl = tokenDto.ReturnUrl
                };

                return new ServiceErrorResponse<AuthResponseDto> { Success = true, Data = data };
            }
            catch (Exception e)
            {

                return new ServiceErrorResponse<AuthResponseDto> { Success = false, Message = $"{e.Message}" };

            }
        }
    }
}
