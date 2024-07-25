using TCommerce.Core.Models.Common;

namespace TCommerce.Core.Models.Banners
{
    public class Banner : BaseEntity
    {
        public string? Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; } = string.Empty;
        public string? ButtonLabel { get; set; } = string.Empty;
        public string? ButtonLink { get; set; } = string.Empty;
        public string? Text { get; set; }
        public int PictureId { get; set; }
    }
}
