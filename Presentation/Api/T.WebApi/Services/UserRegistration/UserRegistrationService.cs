using App.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text;
using T.Library.Model;
using T.Library.Model.Account;
using T.Library.Model.Interface;
using T.Library.Model.Options;
using T.Library.Model.RefreshToken;
using T.Library.Model.Response;
using T.Library.Model.Roles.RoleName;
using T.Library.Model.SendMail;
using T.Library.Model.Users;
using T.WebApi.Database;
using T.WebApi.Extensions;

namespace T.WebApi.Services.UserRegistrations
{
    public interface IUserRegistrationService : IUserRegistrationServiceCommon
    {

    }
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly UserManager<User> _userManager;
        private readonly IOptions<UrlOptions> _urlOptions;
        private readonly IEmailSender _emailService;
        private readonly DatabaseContext _context;
        private readonly SignInManager<User> _signInManager;
        public UserRegistrationService(UserManager<User> userManager, IOptions<UrlOptions> urlOptions, IEmailSender emailService, DatabaseContext context, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _urlOptions = urlOptions;
            _emailService = emailService;
            _context = context;
            _signInManager = signInManager;
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
        public async Task<ServiceResponse<string>> SendResetPasswordEmail(string email)
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
            await _emailService.SendEmailAsync(emailDto);

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
    }
}
