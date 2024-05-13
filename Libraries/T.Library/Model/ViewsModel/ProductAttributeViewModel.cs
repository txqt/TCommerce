using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T.Library.Model.ViewsModel
{
    public class ProductAttributeViewModel
    {
        public int ProductAttributeMappingId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? PicturePath { get; set; }
    }
}
