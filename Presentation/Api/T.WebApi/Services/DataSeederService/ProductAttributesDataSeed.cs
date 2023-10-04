using T.Library.Model;
using T.Library.Model.Common;
using T.WebApi.Extensions;

namespace T.WebApi.Services.DataSeederService
{
    public class ProductAttributesDataSeed : SingletonBase<ProductAttributesDataSeed>
    {
        public static readonly string Color = "Màu sắc";
        public List<ProductAttribute> GetAll()
        {
            return new List<ProductAttribute>()
            {
                new ProductAttribute()
                {
                    Name = Color,
                    Description = $"Thuộc tính {Color}"
                }
            };
        }
    }
}
