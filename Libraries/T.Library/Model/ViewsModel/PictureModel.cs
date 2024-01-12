using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Common;

namespace T.Library.Model.ViewsModel
{
    public partial class PictureModel : BaseEntity
    {
        public string ImageUrl { get; set; }

        public string TitleAttribute { get; set; }

        public string AltAttribute { get; set; }
    }
}
