using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T.Library.Model.Users
{
    public class User : IdentityUser<Guid>
    {
        [MaxLength(30)]
        [Required(ErrorMessage = "Phải nhập tên đệm và tên")]
        public string FirstName { get; set; }

        [MaxLength(30)]
        [Required(ErrorMessage = "Phải nhập họ")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Phải nhập ngày tháng năm sinh")]
        public DateTime Dob { get; set; } // date of birth
        //public List<Order> Orders { get; set; }
        //public List<Cart> Carts { get; set; }
        public string AvatarPath { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedDate { get; set; }

    }
}
