using T.Library.Model.Common;
using T.Library.Model.ViewsModel;

namespace T.Web.Models.Catalog
{
    public partial class CategoryModel : BaseEntity
    {
        public CategoryModel()
        {
            PictureModel = new PictureModel();
            FeaturedProducts = new List<ProductBoxModel>();
            SubCategories = new List<SubCategoryModel>();
            CategoryBreadcrumb = new List<CategoryModel>();
            CatalogProductsModel = new CatalogProductsModel();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }

        public PictureModel PictureModel { get; set; }

        public bool DisplayCategoryBreadcrumb { get; set; }
        public List<CategoryModel> CategoryBreadcrumb { get; set; }

        public List<SubCategoryModel> SubCategories { get; set; }

        public List<ProductBoxModel> FeaturedProducts { get; set; }

        public CatalogProductsModel CatalogProductsModel { get; set; }

        public string JsonLd { get; set; }

        #region Nested Classes

        public partial class SubCategoryModel : BaseEntity
        {
            public SubCategoryModel()
            {
                PictureModel = new PictureModel();
            }

            public string Name { get; set; }

            public string SeName { get; set; }

            public string Description { get; set; }

            public PictureModel PictureModel { get; set; }
        }

        #endregion
    }
}