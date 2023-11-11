using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Banners;
using T.Library.Model.Common;

namespace T.Library.Model.ViewsModel
{
    public class BannerViewModel : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string ButtonLabel { get; set; } = string.Empty;
        public string ButtonLink { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}
