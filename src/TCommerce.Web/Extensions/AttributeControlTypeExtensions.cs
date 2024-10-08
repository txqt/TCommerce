﻿using Microsoft.AspNetCore.Mvc.Rendering;
using TCommerce.Core.Models.Catalogs;

namespace TCommerce.Web.Extensions
{
    public static class AttributeControlTypeExtensions
    {
        public static SelectList ToSelectList(this AttributeControlType selectedValue)
        {
            var values = Enum.GetValues(typeof(AttributeControlType)).Cast<AttributeControlType>();

            var items = values.Select(value => new SelectListItem
            {
                Value = ((int)value).ToString(),
                Text = value.ToString(),
                Selected = (value == selectedValue)
            });

            return new SelectList(items, "Value", "Text");
        }
    }
}
