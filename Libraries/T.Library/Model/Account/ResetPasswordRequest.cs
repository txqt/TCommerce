using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T.Library.Model.Account
{
    public class ResetPasswordRequest
    {
        public required string Token { get; set; }

        [EmailAddress]
        public required string Email { get; set; }

        [StringLength(50, MinimumLength = 5)]
        [DataType(DataType.Password)]
        public required string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 5)]
        [Compare("NewPassword", ErrorMessage = "Hai mật khẩu không giống nhau")]
        public required string ConfirmPassword { get; set; }
    }
}
