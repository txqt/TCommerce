using TCommerce.Core.Models.Common;
using TCommerce.Core.Models.Security;
using TCommerce.Web.Areas.Admin.Models.Datatables;
using TCommerce.Web.Attribute;
using TCommerce.Web.Controllers;

namespace TCommerce.Web.Areas.Admin.Controllers
{
    [CheckPermission(PermissionSystemName.AccessAdminPanel)]
    public class BaseAdminController : BaseController
    {
        protected T ParseQueryStringParameters<T>() where T : QueryStringParameters, new()
        {
            
            var start = int.Parse(Request.Form["start"].FirstOrDefault());
            var length = int.Parse(Request.Form["length"].FirstOrDefault());
            int orderColumnIndex = int.Parse(Request.Form["order[0][column]"]);
            string orderDirection = Request.Form["order[0][dir]"];
            string orderColumnName = Request.Form["columns[" + orderColumnIndex + "][data]"];

            string orderBy = orderColumnName + " " + orderDirection;
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            var parameters = new T
            {
                PageNumber = start / length + 1,
                PageSize = length,
                OrderBy = orderBy,
                SearchText = searchValue
            };

            return parameters;
        }
        protected DataTableResponse<T> ToDatatableReponse<T>(int recordsTotal, int recordsFiltered, List<T> items) where T : BaseEntity, new()
        {
            var draw = int.Parse(Request.Form["draw"].FirstOrDefault());
            return new DataTableResponse<T>
            {
                Draw = draw,
                RecordsTotal = recordsTotal,
                RecordsFiltered = recordsFiltered,
                Data = items
            };
        }
    }
}
