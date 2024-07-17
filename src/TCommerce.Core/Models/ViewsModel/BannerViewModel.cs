using Microsoft.AspNetCore.Http;
using TCommerce.Core.Models.Common;

namespace TCommerce.Core.Models.ViewsModel
{
    public class BannerViewModel : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string ButtonLabel { get; set; } = string.Empty;
        public string ButtonLink { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
