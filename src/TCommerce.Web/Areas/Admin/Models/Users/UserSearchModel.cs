using Microsoft.AspNetCore.Mvc.Rendering;

namespace TCommerce.Web.Areas.Admin.Models.Users
{
    public class UserSearchModel
    {
        public UserSearchModel()
        {
            AvailableUserRoles = new List<SelectListItem>();
            SelectedUserRoleIds = new List<Guid>();
        }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DayOfBirth { get; set; }
        public DateTime? SearchRegistrationDateFrom { get; set; }
        public DateTime? SearchRegistrationDateTo { get; set; }
        public string? Company { get; set; }
        public List<Guid> SelectedUserRoleIds { get; set; }
        public List<SelectListItem> AvailableUserRoles { get; set; }
    }
}
