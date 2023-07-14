using T.Library.Model.Common;
using T.WebApi.Extensions;

namespace T.WebApi.Services.DataSeederService
{
    public class CategoriesDataSeed : SingletonBase<CategoriesDataSeed>
    {
        public static readonly string Fashion = "Thời trang";
        public static readonly string Electronic = "Điện tử";
        public static readonly string Electric_Appliances = "Điện gia dụng";
        public static readonly string Accessory = "Phụ kiện";
        public List<Category> GetAll()
        {
            return new List<Category>()
            {
                new Category()
                {
                    Name = Fashion.ToString(),
                    ShowOnHomepage = true,
                    Published = true,
                    CreatedOnUtc = DateTime.Now,
                },
                new Category()
                {
                    Name = Electronic.ToString(),
                    ShowOnHomepage = true,
                    Published = true,
                    CreatedOnUtc = DateTime.Now,
                },
                new Category()
                {
                    Name = Electric_Appliances.ToString(),
                    ShowOnHomepage = true,
                    Published = true,
                    CreatedOnUtc = DateTime.Now,
                },
                new Category()
                {
                    Name = Accessory.ToString(),
                    ShowOnHomepage = true,
                    Published = true,
                    CreatedOnUtc = DateTime.Now,
                },
            };
        }
        //private static CategoriesSeed _instance;
        //public static CategoriesSeed Instance()
        //{
        //    if (_instance == null)
        //        _instance = new();
        //    return _instance;
        //}
    }
}
