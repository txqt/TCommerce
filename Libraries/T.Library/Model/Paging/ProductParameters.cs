public class ProductParameters : QueryStringParameters
{
    public List<int>? ids { get; set; } = null;
    public List<int>? CategoryIds { get; set; } = null;
    public List<int>? ManufacturerIds { get; set; } = null;
    public bool ExcludeFeaturedProducts { get; set; } = false;
    public decimal? PriceMin { get; set; } = null;
    public decimal? PriceMax { get; set; } = null;
    public int ProductTagId { get; set; } = 0;
    public bool SearchDescriptions { get; set; } = false;
    public bool SearchManufacturerPartNumber { get; set; } = true;
    public bool SearchSku { get; set; } = true;
    public bool SearchProductTags { get; set; } = false;
    public bool ShowHidden { get; set; } = false;
}