using System.ComponentModel.DataAnnotations;

namespace T.Library.Model.ViewsModel
{
    public class AccountInfoModel
    {
        [Required]
        [Display(Name = "Tên")]
        public string? FirstName { get; set; }

        [Required]
        [Display(Name = "Họ")]
        public string? LastName { get; set; }

        [Display(Name = "Sinh nhật")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string? Email { get; set; }
    }
}
