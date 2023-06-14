using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace T.Library.Model.ViewsModel
{
    public class UserModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Tên")]
        [Required(ErrorMessage = "Vui lòng nhập {0}")]
        public string FirstName { get; set; }

        [Display(Name = "Họ")]
        [Required(ErrorMessage = "Vui lòng nhập {0}")]

        public string LastName { get; set; }

        [Display(Name = "Hòm thư")]
        [Required(ErrorMessage = "Vui lòng nhập {0}")]

        public string Email { get; set; }

        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "Vui lòng nhập {0}")]

        public string UserName { get; set; }

        [Display(Name = "Mật khẩu")]
        //[Required(ErrorMessage = "Vui lòng nhập {0}")]

        public string Password { get; set; }

        [Display(Name = "Nhập lại mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu nhập lại không đúng")]
        //[Required(ErrorMessage = "Vui lòng nhập {0}")]

        public string ConfirmPassword { get; set; }

        [Display(Name = "Số điện thoại")]

        public string PhoneNumber { get; set; }

        public IList<string> RoleNames { get; set; }

        public DateTime Dob { get; set; } // date of birth

        public DateTime CreatedDate { get; set; }

        public UserModel()
        {
            RoleNames = new List<string>();
        }
    }
}
