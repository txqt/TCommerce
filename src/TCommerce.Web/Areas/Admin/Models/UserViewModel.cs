using Microsoft.AspNetCore.Mvc.Rendering;
using TCommerce.Core.Models.ViewsModel;

namespace TCommerce.Web.Areas.Admin.Models
{
    public partial class UserViewModel : UserModel
    {
        public IList<SelectListItem> AvailableRoles { get; set; }

        public UserViewModel()
        {
            AvailableRoles = new List<SelectListItem>();
        }
    }
}
