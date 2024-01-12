using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace T.Library.Helpers
{
    public class SlugConverter
    {
        public static string ConvertToSlug(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            // Chuyển về chữ thường
            string slug = input.ToLower();

            // Xóa dấu và ký tự đặc biệt
            slug = RemoveDiacritics(slug);

            // Thay thế các khoảng trắng bằng dấu gạch ngang
            slug = Regex.Replace(slug, @"\s+", "-");

            // Loại bỏ các ký tự không phải chữ cái, số, hoặc dấu gạch ngang
            slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");

            // Loại bỏ các dấu gạch ngang liên tiếp
            slug = Regex.Replace(slug, @"-{2,}", "-");

            return slug;
        }

        private static string RemoveDiacritics(string text)
        {
            string normalized = text.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            foreach (char c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
