using TCommerce.Core.Models.Common;
using TCommerce.Core.Models.ViewsModel;

namespace TCommerce.Web.Models.Catalog
{
    public class ManufacturerModel : BaseEntity
    {
        public ManufacturerModel()
        {
            PictureModel = new PictureModel();
            FeaturedProducts = new List<ProductBoxModel>();
            CatalogProductsModel = new CatalogProductsModel();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }

        public PictureModel PictureModel { get; set; }

        public IList<ProductBoxModel> FeaturedProducts { get; set; }

        public CatalogProductsModel CatalogProductsModel { get; set; }
    }
}
