using System.ComponentModel;
using Microsoft.AspNetCore.Identity;
using T.Library.Model.Users;

namespace T.Library.Model
{
    public class AddUserRoleModel
    {
        public User? user { get; set; }

        [DisplayName("Các role gán cho user")]
        public string[]? RoleNames { get; set; }

        public List<IdentityRoleClaim<string>>? claimsInRole { get; set; }
        public List<IdentityUserClaim<string>>? claimsInUserClaim { get; set; }

    }
}