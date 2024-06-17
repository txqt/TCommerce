using T.Library.Model.Discounts;
using T.Library.Model.Interface;
using T.Library.Model.Response;
using T.Library.Model.ViewsModel;
using T.Web.Helpers;

namespace T.Web.Services.DiscountServices
{
    public interface IDiscountService : IDiscountServiceCommon
    {

    }
    public class DiscountService : HttpClientHelper, IDiscountService
    {
        public DiscountService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<ServiceResponse<bool>> CheckValidDiscountCode(string discountCode)
        {
            return await PostAsJsonAsync<ServiceResponse<bool>>("api/discount/is-valid", discountCode);
        }

        public Task CreateDiscountAsync(Discount discount)
        {
            throw new NotImplementedException();
        }

        public Task DeleteDiscountAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Discount> GetDiscountByCode(string discountCode)
        {
            throw new NotImplementedException();
        }

        public Task UpdateDiscountAsync(Discount discount)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<string>> ValidateDiscountAsync(Discount discount, UserModel user)
        {
            throw new NotImplementedException();
        }
    }
}
