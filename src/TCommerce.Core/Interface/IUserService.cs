using TCommerce.Core.Models.Accounts;
using TCommerce.Core.Models.Common;
using TCommerce.Core.Models.Orders;
using TCommerce.Core.Models.Response;
using TCommerce.Core.Models.Security;
using TCommerce.Core.Models.Users;
using TCommerce.Core.Models.ViewsModel;

namespace TCommerce.Core.Interface
{
    public interface IUserService
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
        Task<ServiceResponse<bool>> UpdateUserAsync(UserModel model, bool requiredRandomPassword = false);
        //Task<List<ShoppingCartItemModel>> GetShoppingCartAsync();
    }
}
