using App.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text;
using T.Library.Model;
using T.Library.Model.Account;
using T.Library.Model.JwtToken;
using T.Library.Model.Options;
using T.Library.Model.RefreshToken;
using T.Library.Model.Response;
using T.Library.Model.Roles.RoleName;
using T.Library.Model.Security;
using T.Library.Model.SendMail;
using T.Library.Model.Users;
using T.WebApi.Database;
using T.WebApi.Extensions;
using T.WebApi.Services.CacheServices;
using T.WebApi.Services.TokenHelpers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace T.WebApi.Services.AccountServices
{
    public interface IAccountService
    {
        Task<LoginResponse<AuthResponseDto>> Login(LoginViewModel login);
        Task<User> FindUserByName(string userName);
        Task<ServiceResponse<AuthResponseDto>> RefreshToken(RefreshTokenDto tokenDto);
        Task<ServiceResponse<bool>> Register(RegisterRequest request);
        Task<bool> Logout(Guid userId);
        Task<ServiceResponse<string>> ConfirmEmail(string userId, string token);
        Task<ServiceResponse<string>> SendChangePasswordEmail(string email);
        Task<ServiceResponse<string>> ResetPassword(ResetPasswordRequest model);
        Task<ServiceResponse<string>> ChangePassword(ChangePasswordRequest model);
    }
    public class AccountService : IAccountService
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IEmailSender _emailService;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly DatabaseContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICacheService _cache;
        private readonly IOptions<AuthorizationOptions> _jwtOptions;
        private readonly IOptions<UrlOptions> _urlOptions;

        public AccountService(UserManager<User> userManager,
                               SignInManager<User> signInManager,
                               IEmailSender emailService,
                               RoleManager<Role> roleManager,
                               DatabaseContext context,
                               IHttpContextAccessor httpContext,
                               IWebHostEnvironment env,
                               ITokenService tokenService,
                               IHttpContextAccessor httpContextAccessor,
                               ICacheService cache,
                               IOptions<AuthorizationOptions> jwtOptions,
                               IOptions<UrlOptions> urlOptions)
        {
            this._emailService = emailService;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _env = env;
            this._tokenService = tokenService;
            _httpContext = httpContext;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _cache = cache;
            _jwtOptions = jwtOptions;
            _urlOptions = urlOptions;
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
                return new ServiceSuccessResponse<string>(_urlOptions.Value.ClientUrl);

            return new ServiceErrorResponse<string>("Email did not confirm");
        }

        public async Task<User> FindUserByName(string userName)
        {
            return await _userManager.FindByNameAsync(userName) ?? throw new ArgumentNullException("Cannot find this user by username");
        }

        public async Task<ServiceResponse<string>> SendChangePasswordEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new ServiceErrorResponse<string>("Tài khoản không tồn tại");

            var confirmEmailToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var validToken = EncodeToken(confirmEmailToken);

            string url = $"{_urlOptions.Value.ClientUrl}/account/reset-password?email={email}&token={validToken}";

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

        public async Task<LoginResponse<AuthResponseDto>> Login(LoginViewModel login)
        {
            if (login.UserNameOrEmail is null || login.Password is null)
                throw new ArgumentNullException("Must enter the necessary information");

            var user = await _userManager.FindByNameAsync(login.UserNameOrEmail);

            if (user is null && AppUtilities.IsValidEmail(login.UserNameOrEmail))
            {
                user = await _userManager.FindByEmailAsync(login.UserNameOrEmail);
            }

            if (user is null || user.UserName is null)
                return new LoginResponse<AuthResponseDto>() { Message = "Tài khoản không tồn tại", Success = false };

            var result = await _signInManager.PasswordSignInAsync(user.UserName, login.Password, login.RememberMe, lockoutOnFailure: true);

            if (result.IsNotAllowed)
                return await HandleNotAllowedUser(user);

            if (user.RequirePasswordChange)
                return await HandlePasswordChangeRequired(user);

            if (result.IsLockedOut)
                return HandleLockedOutUser(user);

            if (!result.Succeeded)
                return new LoginResponse<AuthResponseDto>() { Message = "Tài khoản hoặc mật khẩu không đúng", Success = false };

            return await HandleSuccessfulLogin(user);
        }

        private async Task<LoginResponse<AuthResponseDto>> HandleNotAllowedUser(User user)
        {
            var message = "Tài khoản của bạn đã bị chặn và không thể đăng nhập vào hệ thống";
            if (user.EmailConfirmed == false && user.Email is not null)
            {
                message = await SendConfirmationEmail(user);
            }
            return new LoginResponse<AuthResponseDto>() { Message = $"{message}", Success = false };
        }

        private async Task<string> SendConfirmationEmail(User user)
        {
            var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var encodeToken = EncodeToken(confirmEmailToken);

            string url = $"{_urlOptions.Value.APIUrl}/api/user/confirm-email?userid={user.Id}&token={encodeToken}";

            if (user.Email is null)
                return "Không thể tìm thấy email người dùng";

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
                return "Tài khoản của bạn chưa được xác thực. Hệ thống đã gửi email đến cho bạn, vui lòng kiểm tra email của bạn và làm theo hướng dẫn để hoàn tất quá trình xác thực.\r\n";
            }
            catch
            {
                return "Tài khoản của bạn chưa được xác thực. Không thể gửi email xác nhận, vui lòng thử lại hoặc liên hệ bộ phận kỹ thuật";
            }
        }

        private async Task<LoginResponse<AuthResponseDto>> HandlePasswordChangeRequired(User user)
        {
            if(user.Email is null)
                return new LoginResponse<AuthResponseDto>() {  Success = false, Message = "Không thể tìm thấy email người dùng" };

            await SendChangePasswordEmail(user.Email);

            return new LoginResponse<AuthResponseDto>() { Message = "Tài khoản của bạn cần đổi mật khẩu, vui lòng kiểm tra mail", Success = false };
        }

        private LoginResponse<AuthResponseDto> HandleLockedOutUser(User user)
        {
            var timeLockOutEnd = DateTimeOffset.Now - user.LockoutEnd;

            if(timeLockOutEnd is null)
            {
                return new LoginResponse<AuthResponseDto>() { Message = $"Something went wrong", Success = false };
            }

            return new LoginResponse<AuthResponseDto>() { Message = $"Thử lại sau {timeLockOutEnd.Value}", Success = false };
        }

        private async Task<LoginResponse<AuthResponseDto>> HandleSuccessfulLogin(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            //Create token
            var accessToken = await _tokenService.GenerateAccessToken(user);

            //Create refresh token
            var refreshToken = await _tokenService.GenerateRefreshToken();

            //Save refresh token
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddSeconds(_jwtOptions.Value.RefreshTokenExpirationInSenconds);
            await _userManager.UpdateAsync(user);

            var response = new AuthResponseDto()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };

            return new LoginResponse<AuthResponseDto>() { Success = true, Data = response, RoleNames = roles.ToList() };
        }

        public async Task<bool> Logout(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString()) ??
                throw new ArgumentNullException("Cannot find user by id");

            user.RefreshToken = null;
            _context.Users.Update(user);
            _context.SaveChanges();
            return true;
        }

        public async Task<ServiceResponse<AuthResponseDto>> RefreshToken(RefreshTokenDto tokenDto)
        {
            if (tokenDto is null)
            {
                return new ServiceErrorResponse<AuthResponseDto> { Success = false, Message = "TokenDto is null" };
            }

            var principal = _tokenService.GetPrincipalToken(tokenDto.AccessToken);

            if (principal.Identity == null || principal.Identity.Name is null)
                return new ServiceErrorResponse<AuthResponseDto> { Success = false, Message = "Token does not contain user information" };

            var username = principal.Identity.Name;

            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
                return new ServiceErrorResponse<AuthResponseDto> { Success = false, Message = "User not found" };
            if (user.RefreshToken != tokenDto.RefreshToken)
                return new ServiceErrorResponse<AuthResponseDto> { Success = false, Message = "Invalid refresh token" };
            if (user.RefreshTokenExpiryTime <= DateTime.Now)
                return new ServiceErrorResponse<AuthResponseDto> { Success = false, Message = "Refresh token expired" };

            var accessToken = await _tokenService.GenerateAccessToken(user);

            //var refreshToken = await _tokenService.GenerateRefreshToken();
            //user.RefreshToken = refreshToken;
            //await _userManager.UpdateAsync(user);

            var data = new AuthResponseDto()
            {
                AccessToken = accessToken,
                RefreshToken = tokenDto.RefreshToken,
                //ReturnUrl = tokenDto.ReturnUrl
            };

            return new ServiceErrorResponse<AuthResponseDto> { Success = true, Data = data };
        }

        public async Task<ServiceResponse<bool>> Register(RegisterRequest request)
        {

            if (request.Email is not null)
            {
                if (!AppUtilities.IsValidEmail(request.Email))
                    return new ServiceErrorResponse<bool>("Cần nhập đúng định dạng email");

                if ((await _userManager.FindByEmailAsync(request.Email) != null))
                {
                    return new ServiceErrorResponse<bool>("Email đã tồn tại");
                }
            }

            if ((await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber) != null))
                return new ServiceErrorResponse<bool>("Số điện thoại đã được đăng ký");

            if (request.UserName is not null && AppUtilities.IsValidEmail(request.UserName))
            {
                return new ServiceErrorResponse<bool>("Username không thể là 1 email");
            }

            var user = new User()
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

            if (request.Password is null || request.ConfirmPassword is null && request.Password != request.ConfirmPassword)
                return new ServiceErrorResponse<bool>("Must enter password");

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return new ServiceErrorResponse<bool>(string.Join(", ", errors));
            }
            else
            {
                var defaultrole = await _roleManager.FindByNameAsync(RoleName.Customer);
                if (defaultrole != null && defaultrole.Name != null)
                {
                    IdentityResult roleresult = await _userManager.AddToRoleAsync(user, defaultrole.Name);
                }
                else
                {
                    return new ServiceErrorResponse<bool>("Something went wrong !");
                }
            }

            var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var encodeToken = EncodeToken(confirmEmailToken);

            string url = $"{_urlOptions.Value.APIUrl}/api/account/confirm-email?userid={user.Id}&token={encodeToken}";

            if (user.Email is not null)
            {
                EmailDto emailDto = new EmailDto
                {
                    Subject = "Xác thực email người dùng",
                    Body = $"<h1>Xin chào, {user.LastName + " " + user.FirstName}</h1><br/>"
                + $"<h3>Tài khoản: {user.UserName}</h3></br>"
                + $"<p>Hãy xác nhận email của bạn <a href='{url}'>Bấm vào đây</a></p>",
                    To = user.Email
                };
                await _emailService.SendEmailAsync(emailDto);
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

            user.RequirePasswordChange = false;
            await _context.SaveChangesAsync();

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
            return WebEncoders.Base64UrlEncode(encodedEmailToken);
        }
        public string DecodeToken(string encodeToken)
        {
            var decodedToken = WebEncoders.Base64UrlDecode(encodeToken);
            return Encoding.UTF8.GetString(decodedToken);
        }

        public async Task<ServiceResponse<string>> ChangePassword(ChangePasswordRequest model)
        {
            var userId = model.UserId;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ServiceErrorResponse<string>($"Unable to load user with ID '{userId}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                var errors = changePasswordResult.Errors.Select(e => e.Description);
                return new ServiceErrorResponse<string>(string.Join(", ", errors));
            }
            else
            {
                await _signInManager.RefreshSignInAsync(user);
                return new ServiceSuccessResponse<string>("Your Password has been reset");
            }
        }
    }
}
