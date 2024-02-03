using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using T.Library.Model;
using T.Library.Model.Common;
using T.Library.Model.Security;
using T.Web.Areas.Admin.Models;
using T.Web.Attribute;
using T.Web.Controllers;

namespace T.Web.Areas.Admin.Controllers
{
    [CheckPermission(PermissionSystemName.AccessAdminPanel)]
    public class BaseAdminController : BaseController
    {
        protected T ExtractQueryStringParameters<T>() where T : QueryStringParameters, new()
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
