using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T.Library.Model.Security
{
    public class Role : IdentityRole<Guid>
    {
        public List<PermissionRecordUserRoleMapping> PermissionRecordUserRoleMappings { get; set; }
        public Role(string name) : base(name) { Name = name; }
    }
}
