using TCommerce.Core.Models.Accounts;
using TCommerce.Core.Models.Accounts.Account;
using TCommerce.Core.Models.RefreshToken;
using TCommerce.Core.Models.Response;
using TCommerce.Core.Models.Users;

namespace TCommerce.Core.Interface
{
    public interface IUserRegistrationService
    {
        Task<ServiceResponse<bool>> ConfirmEmail(string userId, string token);
        Task<ServiceResponse<string>> ResetPassword(ResetPasswordRequest model);
        Task<ServiceResponse<string>> ChangePassword(ChangePasswordRequest model);
        Task<ServiceResponse<string>> SendResetPasswordEmail(string email);
        Task<ServiceResponse<AuthResponseDto>> Login(AccessTokenRequestModel loginRequest);
        Task Logout();
        Task<ServiceResponse<string>> SendConfirmationEmail(User user);
    }
}
