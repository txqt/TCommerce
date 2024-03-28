using System.ComponentModel.DataAnnotations;

namespace T.Library.Model.ViewsModel
{
    public class AccountInfoModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
