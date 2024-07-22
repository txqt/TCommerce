using TCommerce.Core.Models.Common;

namespace TCommerce.Web.Models.Catalog
{
    public class ManufacturerNavigationModel
    {
        public ManufacturerNavigationModel()
        {
            Manufacturers = new List<ManufacturerBriefInfoModel>();
        }

        public IList<ManufacturerBriefInfoModel> Manufacturers { get; set; }

        public int TotalManufacturers { get; set; }
    }

    public class ManufacturerBriefInfoModel : BaseEntity
    {
        public string Name { get; set; }

        public string SeName { get; set; }

        public bool IsActive { get; set; }
    }
}
