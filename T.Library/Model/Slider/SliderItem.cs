using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Common;

namespace T.Library.Model.BannerItem
{
    public class SliderItem : BaseEntity
    {
        public string Name { get; set; }
        public string Price { get; set; }
        public string ImgPath { get; set; }
        public int ProductId { get; set; }
    }
}
