using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using TCommerce.Core.Models.Accounts;
using TCommerce.Core.Models.Accounts.Account;
using TCommerce.Core.Models.Options;
using TCommerce.Core.Models.RefreshToken;
using TCommerce.Core.Models.Response;
using TCommerce.Core.Models.SendMail;
using TCommerce.Core.Models.Users;
using TCommerce.Core.Utilities;
using TCommerce.Data;
using TCommerce.Core.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Security.Policy;
using Microsoft.Extensions.Configuration;

namespace TCommerce.Services.UserRegistrations
{
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly UserManager<User> _userManager;
        private readonly IOptions<UrlOptions> _urlOption;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailSender _emailSender;

        public UserRegistrationService(UserManager<User> userManager, IOptions<UrlOptions> urlOption, ApplicationDbContext context, SignInManager<User> signInManager, ITokenService tokenService, IEmailSender emailSender)
        {
            _userManager = userManager;
            _urlOption = urlOption;
            _context = context;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _emailSender = emailSender;
        }

        public async Task<ServiceResponse<AuthResponseDto>> Login(LoginModel loginRequest)
        {
            //return await _tokenService.Create(loginRequest);
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<string>> SendResetPasswordEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new ServiceErrorResponse<string>("Tài khoản không tồn tại");

            var confirmEmailToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var validToken = EncodeToken(confirmEmailToken);

            string url = $"{_urlOption.Value.ClientUrl}/account/reset-password?email={email}&token={validToken}";

            if(user.Email is null)
            {
                return new ServiceErrorResponse<string>("Must have email");
            }

            EmailDto emailDto = new EmailDto
            {
                Subject = "Đặt lại mật khẩu",
                Body = $"<h1>Làm theo hướng dẫn để đặt lại mật khẩu của bạn</h1>" +
                $"<p>Tên đăng nhập của bạn là: </p><h3>{user.UserName}</h3>" +
                $"<p>Để đặt lại mật khẩu <a href='{url}'>Bấm vào đây</a></p>",
                To = user.Email
            };
            await _emailSender.SendEmailAsync(emailDto);

            return new ServiceSuccessResponse<string>("Reset password URL has been sent to the email successfully!");
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
        public async Task<ServiceResponse<string>> ChangePassword(ChangePasswordRequest model)
        {
            var userId = model.UserId;
            var user = await _userManager.FindByIdAsync(userId.ToString());
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
        private string EncodeToken(string normalToken)
        {
            var encodedEmailToken = Encoding.UTF8.GetBytes(normalToken);
            return WebEncoders.Base64UrlEncode(encodedEmailToken);
        }
        private string DecodeToken(string encodeToken)
        {
            var decodedToken = WebEncoders.Base64UrlDecode(encodeToken);
            return Encoding.UTF8.GetString(decodedToken);
        }

        public async Task<ServiceResponse<string>> SendConfirmationEmail(User user)
        {
            if (user == null)
            {
                return new ServiceErrorResponse<string>("User not found.");
            }

            if (user.EmailConfirmed)
            {
                return new ServiceErrorResponse<string>("Email is already confirmed.");
            }

            if (string.IsNullOrEmpty(user.Email))
            {
                return new ServiceErrorResponse<string>("Email is not found.");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var callbackUrl = $"{_urlOption.Value.ClientUrl}/Account/ConfirmEmail?userid={user.Id}&token={EncodeToken(token)}";

            await _emailSender.SendEmailAsync(new EmailDto()
            {
                To = user.Email,
                Body = $"This link is live for 1 hour <br/> Please confirm your account by <a href='{callbackUrl}'>clicking here</a>.",
                Subject = "Confirm TCommerce email"
            });

            return new ServiceSuccessResponse<string>("Success");
        }

        public async Task<ServiceResponse<bool>> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new ServiceErrorResponse<bool>($"Unable to load user with ID '{userId}'.");
            }

            string normalToken = DecodeToken(token);

            var result = await _userManager.ConfirmEmailAsync(user, normalToken);

            if (result.Succeeded)
                return new ServiceSuccessResponse<bool>();

            return new ServiceErrorResponse<bool>("Email did not confirm");
        }
    }
}
