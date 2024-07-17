using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCommerce.Core.Models.Common
{
    public class VietNamCommune : BaseEntity
    {
        public int IdDistrict { get; set; }
        public string? Name { get; set; }
    }
}
