using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Account;
using T.Library.Model.Common;
using T.Library.Model.RefreshToken;
using T.Library.Model.Response;
using T.Library.Model.Security;
using T.Library.Model.Users;
using T.Library.Model.ViewsModel;

namespace T.Library.Model.Interface
{
    public interface IUserServiceCommon
    {
        Task<List<UserModel>> GetAllAsync();
        Task<List<Role>> GetRolesByUserAsync(User user);
        Task<UserModel> Get(Guid id);
        Task<UserModel> GetCurrentUser();
        Task<ServiceResponse<bool>> CreateUserAsync(UserModel model);
        Task<ServiceResponse<bool>> DeleteUserByUserIdAsync(Guid id);
        Task<ServiceResponse<bool>> BanUser(string userId);
        Task<bool> Logout(Guid userId);
        Task<ServiceResponse<bool>> UpdateUserAccountInfo(AccountInfoModel model);
        Task<ServiceResponse<bool>> CreateUserAddressAsync(Address deliveryAddress);
        Task<ServiceResponse<bool>> UpdateUserAddressAsync(Address deliveryAddress);
        Task<ServiceResponse<bool>> DeleteUserAddressAsync(int id);
        Task<List<AddressInfoModel>> GetOwnAddressesAsync();
    }
}
