using App.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Policy;
using System.Text;
using T.Library.Model;
using T.Library.Model.Account;
using T.Library.Model.Enum;
using T.Library.Model.RefreshToken;
using T.Library.Model.Response;
using T.Library.Model.SendMail;
using T.Library.Model.Users;
using T.WebApi.Database.ConfigurationDatabase;
using T.WebApi.Extensions;
using T.WebApi.Helpers.TokenHelpers;
using T.WebApi.Services.CacheServices;

namespace T.WebApi.Services.AccountServices
{
    public interface IAccountService
    {
        Task<LoginResponse<AuthResponseDto>> Login(LoginViewModel login, string? returnUrl);
        Task<User> FindUserByName(string userName);
        Task<ServiceResponse<AuthResponseDto>> RefreshToken(RefreshTokenDto tokenDto);
        Task<ServiceResponse<bool>> Register(RegisterRequest request);
        Task<bool> Logout(Guid userId);
        Task<ServiceResponse<string>> ConfirmEmail(string userId, string token);
        Task<ServiceResponse<string>> ForgotPassword(string email);
        Task<ServiceResponse<string>> ResetPassword(ResetPasswordRequest model);
    }
    public class AccountService : IAccountService
    {
        private readonly IConfiguration _configuration;
        private readonly SignInManager<User> _signInManager;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IEmailSender _emailService;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly DatabaseContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ITokenService _tokenService;
        private readonly ITokenManager _tokenManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICacheService _cache;

        public AccountService(IConfiguration configuration,
                               UserManager<User> userManager,
                               SignInManager<User> signInManager,
                               IEmailSender emailService,
                               RoleManager<Role> roleManager,
                               DatabaseContext context,
                               IHttpContextAccessor httpContext,
                               IWebHostEnvironment env,
                               ITokenService tokenService,
                               IHttpContextAccessor httpContextAccessor,
                               ICacheService cache,
                               ITokenManager tokenManager)
        {
            this._emailService = emailService;
            _configuration = configuration;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _env = env;
            this._tokenService = tokenService;
            _httpContext = httpContext;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _cache = cache;
            _tokenManager = tokenManager;
        }

        public async Task<ServiceResponse<string>> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new ServiceErrorResponse<string>($"Unable to load user with ID '{userId}'.");
            }

            string normalToken = DecodeToken(token);

            var result = await _userManager.ConfirmEmailAsync(user, normalToken);

            if (result.Succeeded)
                return new ServiceSuccessResponse<string>(_configuration.GetSection("Url:ClientUrl").Value);

            return new ServiceErrorResponse<string>("Email did not confirm");
        }

        public async Task<User> FindUserByName(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<ServiceResponse<string>> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new ServiceErrorResponse<string>("Tài khoản không tồn tại");

            var confirmEmailToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var validToken = EncodeToken(confirmEmailToken);

            string url = $"{_configuration["Url:ClientUrl"]}/account/reset-password?email={email}&token={validToken}";

            EmailDto emailDto = new EmailDto
            {
                Subject = "Đặt lại mật khẩu",
                Body = $"<h1>Làm theo hướng dẫn để đặt lại mật khẩu của bạn</h1>" +
                $"<p>Tên đăng nhập của bạn là: </p><h3>{user.UserName}</h3>" +
                $"<p>Để đặt lại mật khẩu <a href='{url}'>Bấm vào đây</a></p>",
                To = user.Email
            };

            try
            {
                await _emailService.SendEmailAsync(emailDto);
            }
            catch
            {
                return new ServiceErrorResponse<string>("Không thể gửi mail đặt lại mật khẩu, vui lòng thử lại hoặc liên hệ bộ phận kỹ thuật");
            }

            return new ServiceSuccessResponse<string>("Reset password URL has been sent to the email successfully!");
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
            {
                var message = "Tài khoản của bạn đã bị chặn và không thể đăng nhập vào hệ thống";
                if(user.EmailConfirmed == false)
                {

                    message = "Tài khoản của bạn chưa được xác thực. Hệ thống đã gửi email đến cho bạn, vui lòng kiểm tra email của bạn và làm theo hướng dẫn để hoàn tất quá trình xác thực.\r\n";

                    var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var encodeToken = EncodeToken(confirmEmailToken);
                    string url = $"{_configuration["Url:ApiUrl"]}/api/user/confirmemail?userid={user.Id}&token={encodeToken}";

                    EmailDto emailDto = new EmailDto
                    {
                        Subject = "Xác thực email người dùng",
                        Body = $"<h1>Xin chào, {user.LastName + " " + user.FirstName}</h1><br/>"
                        + $"<h3>Tài khoản: {user.UserName}</h3></br>"
                        + $"<p>Hãy xác nhận email của bạn <a href='{url}'>Bấm vào đây</a></p>",
                        To = user.Email
                    };
                    try
                    {
                        await _emailService.SendEmailAsync(emailDto);
                    }
                    catch
                    {
                        message = "Tài khoản của bạn chưa được xác thực. Không thể gửi email xác nhận, vui lòng thử lại hoặc liên hệ bộ phận kỹ thuật";
                    }
                    
                }
                return new LoginResponse<AuthResponseDto>() { Message = $"{message}", Success = false };
            }
                

          
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

        public async Task<bool> Logout(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            user.RefreshToken = null;
            _context.Users.Update(user);
            _context.SaveChanges();
            await _tokenManager.DeactivateCurrentAsync();
            return true;
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

        public async Task<ServiceResponse<bool>> Register(RegisterRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user != null)
            {
                return new ServiceErrorResponse<bool>("Tài khoản đã tồn tại");
            }
            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                return new ServiceErrorResponse<bool>("Email đã tồn tại");
            }

            if (await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber) != null)
                return new ServiceErrorResponse<bool>("Số điện thoại đã được đăng ký");

            if (!AppUtilities.IsValidEmail(request.Email))
                return new ServiceErrorResponse<bool>("Cần nhập đúng định dạng email");

            user = new User()
            {
                Dob = request.Dob,
                Email = request.Email,
                NormalizedEmail = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                CreatedDate = AppExtensions.GetDateTimeNow(),
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return new ServiceErrorResponse<bool>(string.Join(", ", errors));
            }
            if (result.Succeeded)
            {
                var defaultrole = _roleManager.FindByNameAsync(RoleName.Customer).Result;
                if (defaultrole != null)
                {
                    IdentityResult roleresult = await _userManager.AddToRoleAsync(user, defaultrole.Name);
                }
            }

            var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodeToken = EncodeToken(confirmEmailToken);
            string url = $"{_configuration["Url:ApiUrl"]}/api/account/confirmemail?userid={user.Id}&token={encodeToken}";

            EmailDto emailDto = new EmailDto
            {
                Subject = "Xác thực email người dùng",
                Body = $"<h1>Xin chào, {user.LastName + " " + user.FirstName}</h1><br/>"
                + $"<h3>Tài khoản: {user.UserName}</h3></br>"
                + $"<p>Hãy xác nhận email của bạn <a href='{url}'>Bấm vào đây</a></p>",
                To = user.Email
            };
            try
            {
                await _emailService.SendEmailAsync(emailDto);
            }
            catch
            {
                return new ServiceErrorResponse<bool>("Không thể gửi email xác nhận, vui lòng thử lại hoặc liên hệ bộ phận kỹ thuật");
            }

            return new ServiceResponse<bool>() { Message = "Vui lòng kiểm tra email để xác nhận !" };
        }

        public async Task<ServiceResponse<string>> ResetPassword(ResetPasswordRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (!AppUtilities.IsValidEmail(model.Email))
                return new ServiceErrorResponse<string>("Cần nhập đúng định dạng email");

            if (user == null)
                return new ServiceErrorResponse<string>("No user associated with email");

            if (model.NewPassword != model.ConfirmPassword)
                return new ServiceErrorResponse<string>("Mật khẩu phải trùng khớp");



            string normalToken = DecodeToken(model.Token);

            try
            {
                var result = await _userManager.ResetPasswordAsync(user, normalToken, model.NewPassword);

                if (result.Succeeded)
                    return new ServiceSuccessResponse<string>("Password has been reset successfully!");
                else
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return new ServiceErrorResponse<string>(string.Join(", ", errors));
                }
            }
            catch
            {
                return new ServiceErrorResponse<string>("Something went wrong !");
            }
        }

        public string EncodeToken(string normalToken)
        {
            var encodedEmailToken = Encoding.UTF8.GetBytes(normalToken);
            return  WebEncoders.Base64UrlEncode(encodedEmailToken); 
        }
        public string DecodeToken(string encodeToken)
        {
            var decodedToken = WebEncoders.Base64UrlDecode(encodeToken);
            return Encoding.UTF8.GetString(decodedToken);
        }
    }
}
