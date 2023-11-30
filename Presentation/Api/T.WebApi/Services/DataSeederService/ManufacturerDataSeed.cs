using T.Library.Model.Catalogs;
using T.WebApi.Extensions;

namespace T.WebApi.Services.DataSeederService
{
    public class ManufacturerDataSeed : SingletonBase<ManufacturerDataSeed>
    {
        public static readonly string GUCCI = "Gucci";
        public static readonly string CHANEL = "Chanel";
        public static readonly string PRADA = "Prada";
        public static readonly string BYZAN = "Byzan";
        public static readonly string DIEN_MAY_XANH = "Điện máy xanh";
        public List<Manufacturer> GetAll()
        {
            return new List<Manufacturer>()
            {
                new Manufacturer()
                {
                    Name = GUCCI,
                    Description = GUCCI,
                    AllowCustomersToSelectPageSize = true,
                    Published = true,
                    Deleted = false,
                    DisplayOrder = 1,
                    CreatedOnUtc = DateTime.Now,
                    UpdatedOnUtc = DateTime.Now,
                    PriceRangeFiltering = false
                },
                new Manufacturer()
                {
                    Name = CHANEL,
                    Description = CHANEL,
                    AllowCustomersToSelectPageSize = true,
                    Published = true,
                    Deleted = false,
                    DisplayOrder = 2,
                    CreatedOnUtc = DateTime.Now,
                    UpdatedOnUtc = DateTime.Now,
                    PriceRangeFiltering = false
                },
                new Manufacturer()
                {
                    Name = PRADA,
                    Description = PRADA,
                    AllowCustomersToSelectPageSize = true,
                    Published = true,
                    Deleted = false,
                    DisplayOrder = 3,
                    CreatedOnUtc = DateTime.Now,
                    UpdatedOnUtc = DateTime.Now,
                    PriceRangeFiltering = false
                },
                new Manufacturer()
                {
                    Name = BYZAN,
                    Description = BYZAN,
                    AllowCustomersToSelectPageSize = true,
                    Published = true,
                    Deleted = false,
                    DisplayOrder = 4,
                    CreatedOnUtc = DateTime.Now,
                    UpdatedOnUtc = DateTime.Now,
                    PriceRangeFiltering = false
                },
                new Manufacturer()
                {
                    Name = DIEN_MAY_XANH,
                    Description = DIEN_MAY_XANH,
                    AllowCustomersToSelectPageSize = true,
                    Published = true,
                    Deleted = false,
                    DisplayOrder = 5,
                    CreatedOnUtc = DateTime.Now,
                    UpdatedOnUtc = DateTime.Now,
                    PriceRangeFiltering = false
                }
            };
        }
    }
}
