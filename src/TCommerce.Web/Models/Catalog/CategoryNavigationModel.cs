using TCommerce.Core.Models.Common;

namespace TCommerce.Web.Models.Catalog
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
