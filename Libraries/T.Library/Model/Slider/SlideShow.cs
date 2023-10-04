using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Common;

namespace T.Library.Model.BannerItem
{
    public class SlideShow : BaseEntity
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public int PictureId { get; set; }
        public Picture Picture { get; set; }
    }
}
