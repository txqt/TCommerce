using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using T.Library.Model.ViewsModel;

namespace T.Web.Areas.Admin.Models
{
    public class UserViewModel : UserModel
    {
        public IList<SelectListItem> AvailableRoles { get; set; }

        public UserViewModel()
        {
            AvailableRoles = new List<SelectListItem>();
            RoleNames = new List<string>();
        }
    }
}
