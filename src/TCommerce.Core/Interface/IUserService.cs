using TCommerce.Core.Models.Accounts;
using TCommerce.Core.Models.Common;
using TCommerce.Core.Models.Orders;
using TCommerce.Core.Models.Paging;
using TCommerce.Core.Models.Response;
using TCommerce.Core.Models.Security;
using TCommerce.Core.Models.Users;
using TCommerce.Core.Models.ViewsModel;

namespace TCommerce.Core.Interface
{
    public interface IUserService
    {
        Task<PagedList<User>> GetAllUsersAsync(UserParameters userParameters);
        Task<List<Role>> GetRolesByUserAsync(User user);
        Task<User> GetUserById(Guid id);
        Task<User> GetCurrentUser();
        Task<ServiceResponse<bool>> CreateUserAsync(User user, List<Guid>? roleIds = null, string? password = "");
        Task<ServiceResponse<bool>> UpdateUserAsync(User model, List<Guid>? roleIds = null, string? password = "", bool requiredRandomPassword = false);
        Task<ServiceResponse<bool>> DeleteUserByUserIdAsync(Guid id);
        Task<ServiceResponse<bool>> BanUser(Guid userId);
        Task<bool> Logout(Guid userId);
        Task<ServiceResponse<bool>> UpdateUserAccountInfo(AccountInfoModel model);
        Task<ServiceResponse<bool>> CreateUserAddressAsync(Address deliveryAddress);
        Task<ServiceResponse<bool>> UpdateUserAddressAsync(Address deliveryAddress);
        Task<ServiceResponse<bool>> DeleteUserAddressAsync(int id);
        Task<List<AddressInfoModel>> GetOwnAddressesAsync();
    }
}
