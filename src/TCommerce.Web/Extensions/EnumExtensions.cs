using Microsoft.AspNetCore.Mvc.Rendering;

namespace TCommerce.Web.Extensions
{
    public static class EnumExtensions
    {
        public static IEnumerable<SelectListItem> ToSelectList<TEnum>() where TEnum : struct, Enum
        {
            return Enum.GetValues(typeof(TEnum))
                       .Cast<TEnum>()
                       .Select(e => new SelectListItem
                       {
                           Text = GetFormattedEnumName(e),
                           Value = Convert.ToInt32(e).ToString()
                       });
        }

        public static string GetFormattedEnumName<TEnum>(this TEnum enumValue) where TEnum : Enum
        {
            string enumName = enumValue.ToString();
            return AddSpacesBeforeUppercase(enumName);
        }

        private static string AddSpacesBeforeUppercase(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var newText = new System.Text.StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                {
                    newText.Append(' ');
                }
                newText.Append(text[i]);
            }
            return newText.ToString();
        }
    }
}
