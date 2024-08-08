using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCommerce.Core.Models.Roles;

namespace TCommerce.Core.Models.Paging
{
    public class UserParameters : QueryStringParameters
    {
        public UserParameters()
        {
            Roles = new List<string>();
        }

        public List<string> Roles { get; set; }
        public DateTime? CreatedFromUtc { get; set; }
        public DateTime? CreatedToUtc { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Company { get; set; }
        public DateTime? DayOfBirth { get; set; }
        public bool Deleted { get; set; } = false;
    }
}
