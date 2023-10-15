using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Response;
using T.Library.Model.Security;
using T.Library.Model.Users;
using T.Library.Model.ViewsModel;

namespace T.Library.Model.Interface
{
    public interface IUserService
    {
        Task<List<UserModel>> GetAllAsync();
        Task<List<Role>> GetRolesByUserAsync(User user);
        Task<ServiceResponse<UserModel>> Get(Guid id);
        Task<ServiceResponse<User>> GetCurrentUser();
        Task<ServiceResponse<bool>> CreateUserAsync(UserModel model);
        Task<ServiceResponse<bool>> UpdateUserAsync(UserModel model);
        Task<ServiceResponse<bool>> DeleteUserByUserIdAsync(Guid id);
        Task<ServiceResponse<bool>> BanUser(string userId);
    }
}
