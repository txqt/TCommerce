using T.Library.Model.Response;
using T.Library.Model.Users;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using T.Library.Model.ViewsModel;
using App.Utilities;
using System.Text;
using T.Library.Model.Security;
using T.Library.Model.Interface;
using T.Library.Model.Roles.RoleName;
using T.WebApi.Database;
using T.Library.Model.Options;
using T.Library.Model.SendMail;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.WebUtilities;
using T.Library.Model.RefreshToken;
using T.Library.Model.Account;
using T.WebApi.Services.TokenServices;

namespace T.WebApi.Services.UserServices
{
    public class UserService : IUserServiceCommon
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailSender _emailService;
        private readonly IOptions<UrlOptions> _urlOptions;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<User> _signInManager;

        public UserService(IMapper mapper, DatabaseContext context, UserManager<User> userManager, RoleManager<Role> roleManager, IHttpContextAccessor httpContextAccessor, IEmailSender emailService, IOptions<UrlOptions> urlOptions, ITokenService tokenService, SignInManager<User> signInManager)
        {
            _mapper = mapper;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
            _urlOptions = urlOptions;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }

        public async Task<ServiceResponse<bool>> CreateUserAsync(UserModel model)
        {
            if (!ValidateEmail(model.Email))
                return new ServiceErrorResponse<bool>("Cần nhập đúng định dạng email");

            if (await UserExistsByEmail(model.Email))
                return new ServiceErrorResponse<bool>("Email đã tồn tại");

            if (await UserExistsByPhoneNumber(model.PhoneNumber))
                return new ServiceErrorResponse<bool>("Số điện thoại đã được đăng ký");

            if (IsEmailUsername(model.UserName))
                return new ServiceErrorResponse<bool>("Username không thể là 1 email");

            var user = _mapper.Map<User>(model);

            user.CreatedDate = DateTime.Now;

            if (model.Password == null)
            {
                model.Password = model.ConfirmPassword = GenerateRandomPassword(length: 6);
            }

            if (ValidatePassword(model.Password, model.ConfirmPassword))
                return new ServiceErrorResponse<bool>("Username không thể là 1 email");

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return new ServiceErrorResponse<bool>(FormatErrors(result.Errors));

            if (!await AssignDefaultRole(user))
                return new ServiceErrorResponse<bool>("Something went wrong!");

            if (model.RoleNames != null && model.RoleNames.Any())
            {
                var selectedRoles = model.RoleNames.Distinct();
                var roleResult = await _userManager.AddToRolesAsync(user, selectedRoles);
                return new ServiceErrorResponse<bool>(FormatErrors(roleResult.Errors));
            }

            return new ServiceSuccessResponse<bool>();
        }

        private string FormatErrors(IEnumerable<IdentityError> errors)
        {
            return string.Join(", ", errors.Select(e => e.Description));
        }

        private bool ValidatePassword(string password, string confirmPassword)
        {
            return password is not null && confirmPassword is not null && password == confirmPassword;
        }

        private bool ValidateEmail(string email)
        {
            return email is not null && AppUtilities.IsValidEmail(email);
        }

        private async Task<bool> UserExistsByEmail(string email)
        {
            return email is not null && await _userManager.FindByEmailAsync(email) != null;
        }

        private async Task<bool> UserExistsByPhoneNumber(string phoneNumber)
        {
            return await _context.Users.AnyAsync(x => x.PhoneNumber == phoneNumber);
        }

        private bool IsEmailUsername(string userName)
        {
            return userName is not null && AppUtilities.IsValidEmail(userName);
        }

        private async Task<bool> AssignDefaultRole(User user)
        {
            var defaultRole = await _roleManager.FindByNameAsync(RoleName.Customer);

            if (defaultRole != null && defaultRole.Name != null)
                return (await _userManager.AddToRoleAsync(user, defaultRole.Name)).Succeeded;

            return false;
        }

        public async Task<ServiceResponse<bool>> UpdateUserAsync(UserModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id.ToString()) ??
                throw new ArgumentNullException($"Cannot find user by id");

            if (!ValidateEmail(model.Email))
                return new ServiceErrorResponse<bool>("Cần nhập đúng định dạng email");

            if (await UserExistsByEmail(model.Email))
                return new ServiceErrorResponse<bool>("Email đã tồn tại");

            if (await UserExistsByPhoneNumber(model.PhoneNumber))
                return new ServiceErrorResponse<bool>("Số điện thoại đã được đăng ký");

            if (IsEmailUsername(model.UserName))
                return new ServiceErrorResponse<bool>("Username không thể là 1 email");

            model.Password = model.ConfirmPassword = GenerateRandomPassword(length: 6);

            if (model.RoleNames != null && model.RoleNames.Any())
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, userRoles);
                var selectedRoles = model.RoleNames.Distinct();
                var roleResult = await _userManager.AddToRolesAsync(user, selectedRoles);
                return new ServiceErrorResponse<bool>(FormatErrors(roleResult.Errors));
            }

            if (!string.IsNullOrEmpty(model.Password))
            {
                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.Password);
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return new ServiceErrorResponse<bool>(FormatErrors(result.Errors));

            return new ServiceSuccessResponse<bool>();
        }

        public async Task<ServiceResponse<bool>> DeleteUserByUserIdAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString()); ;

            if (user == null) { throw new Exception($"Cannot find user: {id}"); }

            user.Deleted = true;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return new ServiceErrorResponse<bool>("Delete user failed");
            }
            return new ServiceSuccessResponse<bool>();
        }

        public async Task<UserModel> Get(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString()) ??
                throw new ArgumentNullException("Cannot find user by that id");

            var roles = await _userManager.GetRolesAsync(user);

            var model = _mapper.Map<UserModel>(user);

            model.RoleNames = roles;

            return model;
        }

        public async Task<List<UserModel>> GetAllAsync()
        {
            var model = _mapper.Map<List<UserModel>>(await _context.Users.Where(x => x.Deleted == false).ToListAsync());
            return model;
        }

        public string GenerateRandomPassword(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
            StringBuilder password = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                int randomIndex = random.Next(validChars.Length);
                password.Append(validChars[randomIndex]);
            }

            return password.ToString();
        }

        public async Task<UserModel> GetCurrentUser()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            // Kiểm tra xem User.Identity.Name có null hay không
            if (httpContext == null || httpContext.User?.Identity?.Name == null)
            {
                return null;
            }

            string username = httpContext.User.Identity.Name;

            var user = await _userManager.FindByNameAsync(username);

            var userModel = _mapper.Map<UserModel>(user);

            return userModel;
        }

        public async Task<List<Role>> GetRolesByUserAsync(User user)
        {
            var list_role = from r in _context.Roles
                            join ur in _context.UserRoles on r.Id equals ur.RoleId
                            where ur.UserId == user.Id
                            select r;
            return await list_role.ToListAsync();
        }

        public async Task<ServiceResponse<bool>> BanUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return new ServiceErrorResponse<bool>("User not found");

            user.Deleted = true;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return new ServiceErrorResponse<bool>("Failed to ban user");

            return new ServiceSuccessResponse<bool>(true);
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
    }
}
