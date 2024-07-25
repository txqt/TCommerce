using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TCommerce.Core.Models.Common;
using TCommerce.Core.Models.Users;

namespace TCommerce.Web.Areas.Admin.Models.Users
{
    public partial class UserModel : ISoftDeletedEntity
    {
        public UserModel()
        {
            RoleIds = new List<Guid>();
            AvailableRoles = new List<SelectListItem>();
        }
        public Guid Id { get; set; }
        public string UserName { get; set; }

        [MaxLength(30)]
        [Required(ErrorMessage = "Phải nhập tên đệm và tên")]
        public string FirstName { get; set; }

        [MaxLength(30)]
        [Required(ErrorMessage = "Phải nhập họ")]
        public string LastName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
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
        [Display(Name = "Danh sách role")]
        public List<Guid> RoleIds { get; set; }
        public IList<SelectListItem> AvailableRoles { get; set; }

        public bool HasShoppingCartItems { get; set; }

        public bool Deleted { get; set; }
    }
}
