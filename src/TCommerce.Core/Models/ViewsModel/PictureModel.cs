using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCommerce.Core.Models.Common;

namespace TCommerce.Core.Models.ViewsModel
{
    public partial class PictureModel : BaseEntity
    {
        public string? ImageUrl { get; set; }

        public string? TitleAttribute { get; set; }

        public string? AltAttribute { get; set; }
    }
}
