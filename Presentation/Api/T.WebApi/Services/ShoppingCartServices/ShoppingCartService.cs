using Microsoft.EntityFrameworkCore;
using T.Library.Model;
using T.Library.Model.Interface;
using T.Library.Model.Orders;
using T.Library.Model.Response;
using T.Library.Model.Users;
using T.Library.Model.ViewsModel;
using T.WebApi.Services.IRepositoryServices;

namespace T.WebApi.Services.ShoppingCartServices
{
    public interface IShoppingCartService : IShoppingCartItemCommon
    {
        Task<ServiceResponse<bool>> CreateAsync(ShoppingCartItem shoppingCartItem);
        Task<List<ShoppingCartItem>> GetShoppingCartAsync(UserModel user, ShoppingCartType? shoppingCartType = null,
        int storeId = 0, int? productId = null, DateTime? createdFromUtc = null, DateTime? createdToUtc = null);
        bool IsUserShoppingCartEmpty(UserModel user);
    }
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IRepository<ShoppingCartItem> _shoppingCartItemService;

        public ShoppingCartService(IRepository<ShoppingCartItem> shoppingCartItemService)
        {
            _shoppingCartItemService = shoppingCartItemService;
        }

        public async Task<ServiceResponse<bool>> CreateAsync(ShoppingCartItem shoppingCartItem)
        {
            await _shoppingCartItemService.CreateAsync(shoppingCartItem);
            return new ServiceSuccessResponse<bool>();
        }

        public async Task<ShoppingCartItem> GetById(int id)
        {
            return await _shoppingCartItemService.GetByIdAsync(id); 
        }

        public virtual async Task<List<ShoppingCartItem>> GetShoppingCartAsync(UserModel user, ShoppingCartType? shoppingCartType = null,
        int storeId = 0, int? productId = null, DateTime? createdFromUtc = null, DateTime? createdToUtc = null)
        {
            ArgumentNullException.ThrowIfNull(user);

            var items = _shoppingCartItemService.Table.Where(sci => sci.UserId == user.Id);

            //filter by type
            if (shoppingCartType.HasValue)
                items = items.Where(item => item.ShoppingCartTypeId == (int)shoppingCartType.Value);

            //filter shopping cart items by product
            if (productId > 0)
                items = items.Where(item => item.ProductId == productId);

            //filter shopping cart items by date
            if (createdFromUtc.HasValue)
                items = items.Where(item => createdFromUtc.Value <= item.CreatedOnUtc);
            if (createdToUtc.HasValue)
                items = items.Where(item => createdToUtc.Value >= item.CreatedOnUtc);

            return await items.ToListAsync();
        }

        public bool IsUserShoppingCartEmpty(UserModel user)
        {
            return !_shoppingCartItemService.Table.Any(sci => sci.UserId == user.Id);
        }
    }
}
