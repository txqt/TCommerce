using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Security;

namespace T.Library.Model.Users
{
    public class Role : IdentityRole<Guid>
    {
        [MaxLength(250)]
        [Required]
        public string Description { get; set; }
        public List<PermissionRecordUserRoleMapping> PermissionRecordUserRoleMappings { get; set; }
        public Role(string name) : base(name) { Name = name; }
    }
}
