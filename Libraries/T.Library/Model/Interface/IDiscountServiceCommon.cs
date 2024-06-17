using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Discounts;
using T.Library.Model.Response;
using T.Library.Model.Users;
using T.Library.Model.ViewsModel;

namespace T.Library.Model.Interface
{
    public interface IDiscountServiceCommon
    {
        Task CreateDiscountAsync(Discount discount);
        Task UpdateDiscountAsync(Discount discount);
        Task DeleteDiscountAsync(int id);
        Task<Discount?> GetDiscountByCode(string discountCode);
        Task<ServiceResponse<string>> ValidateDiscountAsync(Discount discount, UserModel user);
        Task<ServiceResponse<bool>> CheckValidDiscountCode(string discountCode);
    }
}
