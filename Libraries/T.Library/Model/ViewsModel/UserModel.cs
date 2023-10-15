using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using T.Library.Model.Users;

namespace T.Library.Model.ViewsModel
{
    public class UserModel : User
    {
        [Display(Name = "Mật khẩu")]
        public string? Password { get; set; }

        [Display(Name = "Nhập lại mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu nhập lại không đúng")]

        public string? ConfirmPassword { get; set; }

        [Display(Name = "Số điện thoại")]

        public IList<string> RoleNames { get; set; }

        public UserModel()
        {
            RoleNames = new List<string>();
        }
    }
}
