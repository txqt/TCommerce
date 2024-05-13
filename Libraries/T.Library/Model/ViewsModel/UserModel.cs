using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using T.Library.Model.Common;
using T.Library.Model.Users;

namespace T.Library.Model.ViewsModel
{
    public class UserModel : ISoftDeletedEntity
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = null!;

        [MaxLength(30)]
        [Required(ErrorMessage = "Phải nhập tên đệm và tên")]
        public string? FirstName { get; set; }

        [MaxLength(30)]
        [Required(ErrorMessage = "Phải nhập họ")]
        public string? LastName { get; set; }
        public string? NormalizedUserName { get; set; }
        public string? Email { get; set; }
        public string? NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? SecurityStamp { get; set; }
        public string? ConcurrencyStamp { get; set; }
        public string? PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }

        [Display(Name = "Ngày sinh")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        [DataType(DataType.Date)]
        public DateTime Dob { get; set; }

        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = null!;

        [Display(Name = "Nhập lại mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu nhập lại không đúng")]

        public string ConfirmPassword { get; set; } = null!;

        [Display(Name = "Số điện thoại")]

        public IList<string> RoleNames { get; set; }

        public UserModel()
        {
            RoleNames = new List<string>();
        }

        public bool HasShoppingCartItems { get; set; }
        public bool Deleted { get; set; }
    }
}
