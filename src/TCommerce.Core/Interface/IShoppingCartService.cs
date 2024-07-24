using TCommerce.Core.Models.Catalogs;
using TCommerce.Core.Models.Orders;
using TCommerce.Core.Models.Response;
using TCommerce.Core.Models.Users;
using TCommerce.Core.Models.ViewsModel;

namespace TCommerce.Core.Interface
{
    public interface IShoppingCartService
    {
        Task<ShoppingCartItem?> GetById(int id);
        Task<ServiceResponse<bool>> DeleteAsync(int id);
        Task<ServiceResponse<bool>> DeleteBatchAsync(List<int> ids);
        Task<List<ShoppingCartItem>> GetShoppingCartAsync(User user, ShoppingCartType? shoppingCartType = null, int? productId = null, DateTime? createdFromUtc = null, DateTime? createdToUtc = null);
        Task<ServiceResponse<bool>> CreateAsync(ShoppingCartItem model);
        Task<ServiceResponse<bool>> UpdateAsync(ShoppingCartItem model);
        bool IsUserShoppingCartEmpty(User user);
        Task<List<string>> GetShoppingCartItemWarningsAsync(User user, ShoppingCartType shoppingCartType, Product product, string attributesJson, int quantity = 1, bool getStandardWarnings = true, bool getAttributesWarnings = true);
        Task AddToCartAsync(User user, ShoppingCartType cartType, Product product, string? attributeJson = null,
            int quantity = 1);
        Task UpdateCartItemAsync(User user, int cartId, ShoppingCartType cartType, Product product, string? attributeJson = null, int quantity = 1);
        Task ClearShoppingCartAsync(User user);
    }
}
