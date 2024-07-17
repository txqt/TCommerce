using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using TCommerce.Core.Models.Common;

namespace TCommerce.Core.Models.Users
{
    public class User : IdentityUser<Guid>, ISoftDeletedEntity, IEntity
    {
        [MaxLength(30)]
        [Required(ErrorMessage = "Phải nhập tên đệm và tên")]
        public string? FirstName { get; set; }

        [MaxLength(30)]
        [Required(ErrorMessage = "Phải nhập họ")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Phải nhập ngày tháng năm sinh")]
        public DateTime Dob { get; set; } // date of birth
        public string? AvatarPath { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime RefreshTokenExpiryTime { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool RequirePasswordChange { get; set; }

        public bool HasShoppingCartItems { get; set; }

        public int? BillingAddressId { get; set; }

        public int? ShippingAddressId { get; set; }

        public bool Deleted { get; set; }
    }
}
