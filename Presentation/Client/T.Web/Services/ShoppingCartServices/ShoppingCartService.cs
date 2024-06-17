using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections;
using T.Library.Model;
using T.Library.Model.Interface;
using T.Library.Model.Orders;
using T.Library.Model.Response;
using T.Library.Model.ViewsModel;
using T.Web.Extensions;
using T.Web.Helpers;
using T.Web.Services.DiscountServices;

namespace T.Web.Services.ShoppingCartServices
{
    public interface IShoppingCartService : IShoppingCartItemCommon
    {
        Task<List<ShoppingCartItemModel>> GetShoppingCartAsync();
        Task<ServiceResponse<bool>> CreateAsync(ShoppingCartItemModel shoppingCartItem);
        Task<ServiceResponse<bool>> UpdateBatchAsync(List<ShoppingCartItemModel> shoppingCartItems);
        Task<ServiceResponse<bool>> UpdateAsync(ShoppingCartItemModel shoppingCartItem);
        Task<ServiceResponse<bool>> ApplyDiscount(string discountCode);
    }
    public class ShoppingCartService : HttpClientHelper, IShoppingCartService
    {
        private string defaultApi = "api/shopping-cart-items";
        private readonly IDiscountService _discountService;

        public ShoppingCartService(HttpClient httpClient, IDiscountService discountService) : base(httpClient)
        {
            _discountService = discountService;
        }

        public async Task<ServiceResponse<bool>> CreateAsync(ShoppingCartItemModel shoppingCartItem)
        {
            return await PostAsJsonAsync<ServiceResponse<bool>>(defaultApi, shoppingCartItem);
        }
        public async Task<ServiceResponse<bool>> UpdateBatchAsync(List<ShoppingCartItemModel> shoppingCartItems)
        {
            return await PutAsJsonAsync<ServiceResponse<bool>>($"{defaultApi}/batch", shoppingCartItems);
        }

        public Task<ShoppingCartItem> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ShoppingCartItemModel>> GetShoppingCartAsync()
        {
            return await GetAsync<List<ShoppingCartItemModel>>($"{defaultApi}/me");
        }

        public async Task<ServiceResponse<bool>> DeleteAsync(int id)
        {
            return await DeleteAsync<ServiceResponse<bool>>($"{defaultApi}/{id}");
        }

        public async Task<ServiceResponse<bool>> UpdateAsync(ShoppingCartItemModel shoppingCartItem)
        {
            return await PutAsJsonAsync<ServiceResponse<bool>>(defaultApi, shoppingCartItem);
        }

        public async Task<ServiceResponse<bool>> DeleteBatchAsync(List<int> ids)
        {
            return await DeleteWithDataAsync<ServiceResponse<bool>>($"{defaultApi}/delete-list", ids);
        }

        public Task<List<ShoppingCartItem>> GetShoppingCartAsync(UserModel user, ShoppingCartType? shoppingCartType = null, int? productId = null, DateTime? createdFromUtc = null, DateTime? createdToUtc = null)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<bool>> ApplyDiscount(string discountCode)
        {
            var validCheck = await _discountService.CheckValidDiscountCode(discountCode);
            if (validCheck is not null && validCheck.Success)
            {
                return new ServiceSuccessResponse<bool>();
            }
            return new ServiceErrorResponse<bool>();
        }
    }
}
