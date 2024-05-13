using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Account;
using T.Library.Model.Response;

namespace T.Library.Model.Interface
{
    public interface IUserRegistrationServiceCommon
    {
        //Task<ServiceResponse<bool>> ConfirmEmail(string userId, string token);
        Task<ServiceResponse<string>> ResetPassword(ResetPasswordRequest model);
        Task<ServiceResponse<string>> ChangePassword(ChangePasswordRequest model);
        Task<ServiceResponse<string>> SendResetPasswordEmail(string email);
    }
}
