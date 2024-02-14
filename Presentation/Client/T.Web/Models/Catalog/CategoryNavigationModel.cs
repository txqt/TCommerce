using T.Library.Model.Common;

namespace T.Web.Models.Catalog
{
    public class CategoryNavigationModel : BaseEntity
    {
        public CategoryNavigationModel()
        {
            Categories = new List<CategorySimpleModel>();
        }

        public int CurrentCategoryId { get; set; }
        public List<CategorySimpleModel> Categories { get; set; }
    }
}
