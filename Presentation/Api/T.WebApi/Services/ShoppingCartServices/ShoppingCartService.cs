using Microsoft.EntityFrameworkCore;
using T.Library.Model;
using T.Library.Model.Interface;
using T.Library.Model.Orders;
using T.Library.Model.Response;
using T.Library.Model.Users;
using T.Library.Model.ViewsModel;
using T.WebApi.Services.IRepositoryServices;
using T.WebApi.Services.ProductServices;
using static T.Library.Model.ViewsModel.ShoppingCartItemModel;

namespace T.WebApi.Services.ShoppingCartServices
{
    public interface IShoppingCartService : IShoppingCartItemCommon
    {
        Task<ServiceResponse<bool>> CreateAsync(ShoppingCartItem shoppingCartItem);
        Task<ServiceResponse<bool>> UpdateAsync(ShoppingCartItem shoppingCartItem);
        Task<List<ShoppingCartItem>> GetShoppingCartAsync(UserModel user, ShoppingCartType? shoppingCartType = null,
        int storeId = 0, int? productId = null, DateTime? createdFromUtc = null, DateTime? createdToUtc = null);
        bool IsUserShoppingCartEmpty(UserModel user);
        Task<List<string>> ValidateCart(List<ProductAttributeMapping> attributeMappings, List<SelectedAttribute> selectedAttributes);
    }
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IRepository<ShoppingCartItem> _shoppingCartItemService;
        private readonly IProductAttributeService _productAttribute;

        public ShoppingCartService(IRepository<ShoppingCartItem> shoppingCartItemService, IProductAttributeService productAttribute)
        {
            _shoppingCartItemService = shoppingCartItemService;
            _productAttribute = productAttribute;
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

        //public virtual async Task<ShoppingCartItem> FindShoppingCartItemInTheCartAsync(List<ShoppingCartItem> shoppingCart,
        //ShoppingCartType shoppingCartType,
        //string attributesJson = "",
        //decimal customerEnteredPrice = decimal.Zero)
        //{
        //    ArgumentNullException.ThrowIfNull(shoppingCart);

        //    return await shoppingCart.Where(sci => sci.ShoppingCartType == shoppingCartType).FirstOrDefaultAsync(x => x.AttributeJson.Equals(attributesJson));
        //}

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
    }
}
