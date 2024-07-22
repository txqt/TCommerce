using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCommerce.Core.Models.Common;

namespace TCommerce.Core.Models.Configuration
{
    public class Setting : BaseEntity
    {
        public string? Name { get; set; }
        public string? Value { get; set; }
    }
}
