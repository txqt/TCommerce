using System.Collections;
using System.Net;
using System.Text;

namespace T.Web.Extensions
{
    public static class CommonExtensions
    {
        public static string ToQueryString(object obj, string prefix = "")
        {
            if (obj == null)
            {
                return string.Empty;
            }

            var queryString = new List<string>();

            if (obj is IEnumerable enumerable && !(obj is string))
            {
                int index = 0;
                foreach (var item in enumerable)
                {
                    if (item != null)
                    {
                        queryString.Add(ToQueryString(item, $"{prefix}[{index}]"));
                        index++;
                    }
                }
                // Handle empty collections
                if (index == 0)
                {
                    queryString.Add($"{prefix}=");
                }
            }
            else
            {
                var properties = obj.GetType().GetProperties();
                foreach (var property in properties)
                {
                    // Skip indexed properties
                    if (property.GetIndexParameters().Length > 0)
                    {
                        continue;
                    }

                    var value = property.GetValue(obj, null);
                    if (value == null)
                    {
                        continue;
                    }

                    var propertyName = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}.{property.Name}";

                    if (value is string || value.GetType().IsValueType)
                    {
                        queryString.Add($"{propertyName}={System.Net.WebUtility.UrlEncode(value.ToString())}");
                    }
                    else
                    {
                        queryString.Add(ToQueryString(value, propertyName));
                    }
                }
            }

            return string.Join("&", queryString);
        }
    }
}
