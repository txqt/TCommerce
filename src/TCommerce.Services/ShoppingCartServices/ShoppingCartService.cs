using Microsoft.EntityFrameworkCore;
using TCommerce.Core.Extensions;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Core.Models.Orders;
using TCommerce.Core.Models.Response;
using TCommerce.Core.Models.Users;
using TCommerce.Core.Models.ViewsModel;
using TCommerce.Services.IRepositoryServices;
using TCommerce.Services.ManufacturerServices;
using TCommerce.Services.ProductServices;

namespace TCommerce.Services.ShoppingCartServices
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IRepository<ShoppingCartItem> _shoppingCartItemRepository;
        private readonly IProductAttributeService _productAttribute;
        private readonly IUserService _userService;
        private readonly IProductAttributeConverter _productAttributeConverter;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerServices;

        public ShoppingCartService(IRepository<ShoppingCartItem> shoppingCartItemService, IProductAttributeService productAttribute, IUserService userService, IProductAttributeConverter productAttributeConverter, ICategoryService categoryService, IManufacturerService manufacturerServices)
        {
            _shoppingCartItemRepository = shoppingCartItemService;
            _productAttribute = productAttribute;
            _userService = userService;
            _productAttributeConverter = productAttributeConverter;
            _categoryService = categoryService;
            _manufacturerServices = manufacturerServices;
        }

        public async Task<ServiceResponse<bool>> CreateAsync(ShoppingCartItem shoppingCartItem)
        {
            shoppingCartItem.CreatedOnUtc = DateTime.Now;
            await _shoppingCartItemRepository.CreateAsync(shoppingCartItem);
            return new ServiceSuccessResponse<bool>();
        }

        public async Task<ShoppingCartItem?> GetById(int id)
        {
            return await _shoppingCartItemRepository.GetByIdAsync(id);
        }

        public virtual async Task<List<ShoppingCartItem>> GetShoppingCartAsync(UserModel user, ShoppingCartType? shoppingCartType = null, int? productId = null, DateTime? createdFromUtc = null, DateTime? createdToUtc = null)
        {
            ArgumentNullException.ThrowIfNull(user);

            var items = _shoppingCartItemRepository.Table.Where(sci => sci.UserId == user.Id);

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
            return !_shoppingCartItemRepository.Table.Any(sci => sci.UserId == user.Id);
        }

        public async Task<ShoppingCartItem?> FindShoppingCartItemInTheCartAsync(List<ShoppingCartItem> shoppingCart,
        ShoppingCartType shoppingCartType,
        string? attributesJson = "")
        {
            ArgumentNullException.ThrowIfNull(shoppingCart);

            return await shoppingCart.Where(sci => sci.ShoppingCartType == shoppingCartType)
                .FirstOrDefaultAsync(x => x.AttributeJson is not null && x.AttributeJson.Equals(attributesJson));
        }

        public async Task<ServiceResponse<bool>> UpdateAsync(ShoppingCartItem shoppingCartItem)
        {
            shoppingCartItem.UpdatedOnUtc = DateTime.Now;
            await _shoppingCartItemRepository.UpdateAsync(shoppingCartItem);
            return new ServiceSuccessResponse<bool>();
        }



        public async Task<ServiceResponse<bool>> DeleteAsync(int id)
        {
            await _shoppingCartItemRepository.DeleteAsync(id);
            await UpdateUserCartItemState(await _userService.GetCurrentUser());
            return new ServiceSuccessResponse<bool>();
        }

        public async Task AddToCartAsync(UserModel user, ShoppingCartType cartType, Product product, string? attributeJson = null, int quantity = 1)
        {
            ArgumentNullException.ThrowIfNull(user);
            ArgumentNullException.ThrowIfNull(product);

            var carts = await GetShoppingCartAsync(user, cartType, product.Id);
            var shoppingCartItem = await FindShoppingCartItemInTheCartAsync(carts,
            cartType, attributeJson);

            if (shoppingCartItem is not null)
            {
                shoppingCartItem.Quantity += quantity;
                shoppingCartItem.AttributeJson = attributeJson!;
                await UpdateAsync(shoppingCartItem);
            }
            else
            {
                shoppingCartItem = new ShoppingCartItem();
                shoppingCartItem.ShoppingCartType = cartType;
                shoppingCartItem.Quantity = quantity;
                shoppingCartItem.AttributeJson = attributeJson!;
                shoppingCartItem.UserId = user.Id;
                shoppingCartItem.ProductId = product.Id;
                shoppingCartItem.CreatedOnUtc = DateTime.UtcNow;
                shoppingCartItem.UpdatedOnUtc = DateTime.UtcNow;

                await CreateAsync(shoppingCartItem);
            }
            await UpdateUserCartItemState(user);
        }

        private async Task UpdateUserCartItemState(UserModel user)
        {
            if (user is not null)
            {
                var hasShoppingCartItems = !IsUserShoppingCartEmpty(user);
                if (hasShoppingCartItems != user.HasShoppingCartItems)
                {
                    user.HasShoppingCartItems = hasShoppingCartItems;
                    await _userService.UpdateUserAsync(user, false);
                }
            }
        }

        public async Task UpdateCartItemAsync(UserModel user, int cartId, ShoppingCartType cartType, Product product, string? attributeJson = null, int quantity = 1)
        {
            ArgumentNullException.ThrowIfNull(user);

            var carts = await GetShoppingCartAsync(user, cartType, product.Id);
            var otherCartWithSameParameters = await FindShoppingCartItemInTheCartAsync(carts,
            cartType, attributeJson);
            var currentCart = (await GetById(cartId))!;

            //check if that other cart is current cart
            if (otherCartWithSameParameters is not null && otherCartWithSameParameters.Id == currentCart.Id)
            {
                otherCartWithSameParameters = null;
            }

            if (otherCartWithSameParameters is not null && otherCartWithSameParameters.UserId == user.Id)
            {
                otherCartWithSameParameters.Quantity += quantity;
                otherCartWithSameParameters.AttributeJson = attributeJson!;
                if (currentCart is not null && currentCart.Id != otherCartWithSameParameters.Id)
                {
                    await DeleteAsync(currentCart.Id);
                    await UpdateAsync(otherCartWithSameParameters);
                }
            }
            else
            {
                currentCart.ShoppingCartType = cartType;
                currentCart.Quantity = quantity;
                currentCart.AttributeJson = attributeJson!;
                await UpdateAsync(currentCart);
            }

            await UpdateUserCartItemState(user);
        }

        public async Task<ServiceResponse<bool>> DeleteBatchAsync(List<int> ids)
        {
            await _shoppingCartItemRepository.BulkDeleteAsync(ids);
            await UpdateUserCartItemState(await _userService.GetCurrentUser());
            return new ServiceSuccessResponse<bool>();
        }

        public async Task ClearShoppingCartAsync(UserModel user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var carts = await GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart);

            await _shoppingCartItemRepository.BulkDeleteAsync(carts.Select(x=>x.Id));

            await UpdateUserCartItemState(user);
        }

        #region Warnings
        public async Task<List<string>> GetShoppingCartItemWarningsAsync(UserModel user, ShoppingCartType shoppingCartType, Product product, string attributesJson, int quantity = 1, bool getStandardWarnings = true, bool getAttributesWarnings = true)
        {
            var warnings = new List<string>();

            if (getStandardWarnings)
            {
                warnings.AddRange(GetStandardWarningsAsync(user, shoppingCartType, product, attributesJson, quantity));
            }

            if (getAttributesWarnings)
            {
                warnings.AddRange(await GetShoppingCartItemAttributeWarningsAsync(user, product, attributesJson));
            }

            return warnings;
        }

        protected virtual List<string> GetStandardWarningsAsync(UserModel user, ShoppingCartType shoppingCartType, Product product,
        string attributesJson, int quantity)
        {
            List<string> warnings = new List<string>();

            ArgumentNullException.ThrowIfNull(user);

            ArgumentNullException.ThrowIfNull(product);

            //deleted
            if (product.Deleted)
            {
                warnings.Add($"Sản phẩm [{product.Name}] đã bị xóa (tạm thời)");
                return warnings;
            }

            //published
            if (!product.Published)
            {
                warnings.Add($"Sản phẩm [{product.Name}] chưa được xuất bản");
            }

            if (shoppingCartType == ShoppingCartType.ShoppingCart && product.DisableBuyButton)
            {
                warnings.Add($"Sản phẩm [{product.Name}] này không được thêm vào giỏ hàng nữa");
            }

            if (shoppingCartType == ShoppingCartType.Wishlist && product.DisableWishlistButton)
            {
                warnings.Add($"Sản phẩm [{product.Name}] này không được thêm vào wishlist nữa");
            }

            if (quantity > product.StockQuantity)
            {
                warnings.Add(string.Format($"{product.Name} chỉ còn {product.StockQuantity}"));
            }

            if (quantity < product.OrderMinimumQuantity)
            {
                warnings.Add(string.Format($"Số lượng {product.Name} phải chọn ít nhất là {product.OrderMinimumQuantity}"));
            }

            if (quantity > product.OrderMaximumQuantity)
            {
                warnings.Add(string.Format(string.Format($"Số lượng [{product.Name}] nhiều nhất là {product.OrderMaximumQuantity}")));
            }

            var availableStartDateError = false;
            if (product.AvailableStartDateTimeUtc.HasValue)
            {
                var availableStartDateTime = DateTime.SpecifyKind(product.AvailableStartDateTimeUtc.Value, DateTimeKind.Utc);
                if (availableStartDateTime.CompareTo(DateTime.UtcNow) > 0)
                {
                    warnings.Add("Sản phẩm này chưa có sẵn");
                    availableStartDateError = true;
                }
            }

            if (!product.AvailableEndDateTimeUtc.HasValue || availableStartDateError)
                return warnings;

            var availableEndDateTime = DateTime.SpecifyKind(product.AvailableEndDateTimeUtc.Value, DateTimeKind.Utc);
            if (availableEndDateTime.CompareTo(DateTime.UtcNow) < 0)
            {
                warnings.Add("Sản phẩm này chưa có sẵn");
            }

            return warnings;
        }

        public virtual async Task<List<string>> GetShoppingCartItemAttributeWarningsAsync(UserModel user,
        Product product,
        string attributesJson = "")
        {
            ArgumentNullException.ThrowIfNull(product);

            var warnings = new List<string>();

            //convert json string of user selected attributes
            var selectedAttributes = _productAttributeConverter.ConvertToObject(attributesJson);

            //all attributes this product has
            var attributeMappings = await _productAttribute.GetProductAttributesMappingByProductIdAsync(product.Id);

            //get the list of selected ProductAttributeMappingId
            //if selectedAttributes is null, then create a new int list so that the code "selectedAttributesIds.Contains(attributeMapping.Id)" does not error.
            var selectedAttributesIds = selectedAttributes?.Select(x => x.ProductAttributeMappingId).ToList() ?? new List<int>();

            //get a list of attributes that are not selected but are present in that product
            var unselectedAttributeMappings = attributeMappings
            .Where(attributeMapping => !selectedAttributesIds.Contains(attributeMapping.Id))
            .ToList();

            foreach (var attributeMapping in unselectedAttributeMappings)
            {
                if (attributeMapping.IsRequired)
                {
                    var productAttribute = await _productAttribute.GetProductAttributeByIdAsync(attributeMapping.ProductAttributeId);
                    if (productAttribute != null)
                    {
                        var attributeName = productAttribute.Name;
                        warnings.Add($"Sản phẩm [{product.Name}] phải chọn [{attributeName}]");
                    }
                }
            }


            return warnings;
        }
        #endregion
    }
}