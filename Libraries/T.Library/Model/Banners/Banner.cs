using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Common;

namespace T.Library.Model.Banners
{
    public class Banner : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string ButtonLabel { get; set; } = string.Empty;
        public string ButtonLink { get; set; } = string.Empty;
        public int ButtonPositionX { get; set; }
        public int ButtonPositionY { get; set; }
    }
}
