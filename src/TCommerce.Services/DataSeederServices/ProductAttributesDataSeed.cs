﻿using TCommerce.Core.Extensions;
using TCommerce.Core.Models.Catalogs;

namespace TCommerce.Services.DataSeederServices
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
