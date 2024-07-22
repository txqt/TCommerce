namespace TCommerce.Web.Models.Catalog
{
    public class SearchBoxModel
    {
        public bool AutoCompleteEnabled { get; set; }
        public bool ShowProductImagesInSearchAutoComplete { get; set; }
        public int SearchTermMinimumLength { get; set; }
        public bool ShowSearchBox { get; set; }
    }
}
