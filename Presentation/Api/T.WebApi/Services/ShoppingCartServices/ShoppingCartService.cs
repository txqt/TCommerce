using Microsoft.EntityFrameworkCore;
using T.Library.Model;
using T.Library.Model.Interface;
using T.Library.Model.Orders;
using T.Library.Model.Response;
using T.Library.Model.Users;
using T.Library.Model.ViewsModel;
using T.WebApi.Extensions;
using T.WebApi.Services.IRepositoryServices;
using T.WebApi.Services.ProductServices;
using T.WebApi.Services.UserServices;
using static T.Library.Model.ViewsModel.ShoppingCartItemModel;

namespace T.WebApi.Services.ShoppingCartServices
{
    public interface IShoppingCartService : IShoppingCartItemCommon
    {
        Task<ServiceResponse<bool>> CreateAsync(ShoppingCartItem shoppingCartItem);
        Task<ServiceResponse<bool>> UpdateAsync(ShoppingCartItem shoppingCartItem);
        Task<List<ShoppingCartItem>> GetShoppingCartAsync(UserModel user, ShoppingCartType? shoppingCartType = null, int? productId = null, DateTime? createdFromUtc = null, DateTime? createdToUtc = null);
        bool IsUserShoppingCartEmpty(UserModel user);
        Task<List<string>> ValidateCart(List<ProductAttributeMapping> attributeMappings, List<SelectedAttribute> selectedAttributes);
        Task AddToCartAsync(UserModel user, ShoppingCartType cartType, Product product, string attributeJson = null,
            int quantity = 1);
        Task UpdateCartItemAsync(UserModel user, int cartId, ShoppingCartType cartType, string attributeJson = null, int quantity = 1);
    }
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IRepository<ShoppingCartItem> _shoppingCartItemService;
        private readonly IProductAttributeService _productAttribute;
        private readonly IUserService _userService;

        public ShoppingCartService(IRepository<ShoppingCartItem> shoppingCartItemService, IProductAttributeService productAttribute, IUserService userService)
        {
            _shoppingCartItemService = shoppingCartItemService;
            _productAttribute = productAttribute;
            _userService = userService;
        }

        public async Task<ServiceResponse<bool>> CreateAsync(ShoppingCartItem shoppingCartItem)
        {
            shoppingCartItem.CreatedOnUtc = DateTime.Now;
            await _shoppingCartItemService.CreateAsync(shoppingCartItem);
            return new ServiceSuccessResponse<bool>();
        }

        public async Task<ShoppingCartItem> GetById(int id)
        {
            return await _shoppingCartItemService.GetByIdAsync(id);
        }

        public virtual async Task<List<ShoppingCartItem>> GetShoppingCartAsync(UserModel user, ShoppingCartType? shoppingCartType = null, int? productId = null, DateTime? createdFromUtc = null, DateTime? createdToUtc = null)
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

        public async Task<ShoppingCartItem> FindShoppingCartItemInTheCartAsync(List<ShoppingCartItem> shoppingCart,
        ShoppingCartType shoppingCartType,
        string attributesJson = "",
        decimal customerEnteredPrice = decimal.Zero)
        {
            ArgumentNullException.ThrowIfNull(shoppingCart);

            return await shoppingCart.Where(sci => sci.ShoppingCartType == shoppingCartType)
                .FirstOrDefaultAsync(x => x.AttributeJson.Equals(attributesJson));
        }

        public async Task<ServiceResponse<bool>> UpdateAsync(ShoppingCartItem shoppingCartItem)
        {
            shoppingCartItem.UpdatedOnUtc = DateTime.Now;
            await _shoppingCartItemService.UpdateAsync(shoppingCartItem);
            return new ServiceSuccessResponse<bool>();
        }

        public async Task<List<string>> ValidateCart(List<ProductAttributeMapping> attributeMappings, List<SelectedAttribute> selectedAttributes)
        {
            var warnings = new List<string>();

            var selectedAttributesId = selectedAttributes.Select(x => x.ProductAttributeMappingId).ToList();

            var unselectedAttributeMappings = attributeMappings
            .Where(attributeMapping => !selectedAttributesId.Contains(attributeMapping.Id))
            .ToList();

            foreach (var attributeMapping in unselectedAttributeMappings)
            {
                if (attributeMapping.IsRequired)
                {
                    var attributeName = (await _productAttribute.GetProductAttributeByIdAsync(attributeMapping.ProductAttributeId)).Name;
                    warnings.Add($"Phải chọn {attributeName}");
                }
            }

            return warnings;
        }

        public async Task<ServiceResponse<bool>> DeleteAsync(int id)
        {
            await _shoppingCartItemService.DeleteAsync(id);
            return new ServiceSuccessResponse<bool>();
        }

        public async Task AddToCartAsync(UserModel user, ShoppingCartType cartType, Product product, string attributeJson = null, int quantity = 1)
        {
            ArgumentNullException.ThrowIfNull(user);
            ArgumentNullException.ThrowIfNull(product);

            var cart = await GetShoppingCartAsync(user, cartType, product.Id);
            var shoppingCartItem = await FindShoppingCartItemInTheCartAsync(cart,
            cartType, attributeJson);

            if (shoppingCartItem != null)
            {
                shoppingCartItem.Quantity += quantity;
                await UpdateAsync(shoppingCartItem);
            }
            else
            {
                var now = DateTime.UtcNow;
                shoppingCartItem = new ShoppingCartItem
                {
                    ShoppingCartType = cartType,
                    ProductId = product.Id,
                    AttributeJson = attributeJson,
                    Quantity = quantity,
                    CreatedOnUtc = now,
                    UpdatedOnUtc = now,
                    UserId = user.Id
                };
                await CreateAsync(shoppingCartItem);
            }
            await UpdateUserCartItemState();
        }

        private async Task UpdateUserCartItemState()
        {
            var customer = await _userService.GetCurrentUser();

            if (customer is not null)
            {
                var hasShoppingCartItems = !IsUserShoppingCartEmpty(customer);
                if (hasShoppingCartItems != customer.HasShoppingCartItems)
                {
                    customer.HasShoppingCartItems = hasShoppingCartItems;
                    await _userService.UpdateUserAsync(customer, false);
                }
            }
        }

        public async Task UpdateCartItemAsync(UserModel user, int cartId, ShoppingCartType cartType, string attributeJson = null, int quantity = 1)
        {
            ArgumentNullException.ThrowIfNull(user);

            var shoppingCartItem = await GetById(cartId);

            if (shoppingCartItem == null || shoppingCartItem.UserId != user.Id)
                throw new ArgumentNullException();

            shoppingCartItem.ShoppingCartType = cartType;
            shoppingCartItem.Quantity = quantity;
            shoppingCartItem.AttributeJson = attributeJson;

            await UpdateAsync(shoppingCartItem);
        }
    }
}
