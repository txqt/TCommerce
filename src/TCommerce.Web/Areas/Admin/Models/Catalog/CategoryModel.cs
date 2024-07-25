using Microsoft.AspNetCore.Mvc.Rendering;
using TCommerce.Core.Models.Common;
using TCommerce.Core.Models.ViewsModel;
using TCommerce.Web.Models.Catalog;
using TCommerce.Web.Models;

namespace TCommerce.Web.Areas.Admin.Models.Catalog
{
    public class CategoryModel : BaseEntity
    {
        public CategoryModel()
        {
            AvailableCategories = new List<SelectListItem>();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }
        public int ParentCategoryId { get; set; }
        public string ParentCategoryName { get; set; }
        public bool Published { get; set; } = true;
        public int DisplayOrder { get; set; }
        public bool AllowCustomersToSelectPageSize { get; set; } = true;
        public int PageSize { get; set; } = 10;
        public string PageSizeOptions { get; set; }
        public bool ShowOnHomepage { get; set; }
        public bool IncludeInTopMenu { get; set; }
        public List<SelectListItem> AvailableCategories { get; set; }
    }
}
