using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCommerce.Core.Models.Common;

namespace TCommerce.Core.Models.Security
{
    public class Role : IdentityRole<Guid>, IEntity
    {
        public List<PermissionRecordUserRoleMapping>? PermissionRecordUserRoleMappings { get; set; }
        public Role(string name) : base(name) { Name = name; }
    }
}
