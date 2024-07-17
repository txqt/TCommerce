using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCommerce.Core.Models.Common
{
    public class VietNamDistrict : BaseEntity
    {
        public int IdProvince { get; set; }
        public string? Name { get; set; }
    }
}
