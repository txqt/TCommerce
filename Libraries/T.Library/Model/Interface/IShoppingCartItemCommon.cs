using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Orders;
using T.Library.Model.Response;

namespace T.Library.Model.Interface
{
    public interface IShoppingCartItemCommon
    {
        Task<ShoppingCartItem> GetById(int id);
        Task<ServiceResponse<bool>> DeleteAsync(int id);
        Task<ServiceResponse<bool>> DeleteBatchAsync(List<int> ids);
    }
}
