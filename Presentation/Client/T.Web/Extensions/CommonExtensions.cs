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
            var properties = obj.GetType().GetProperties();

            foreach (var property in properties)
            {
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
                else if (value is IEnumerable enumerable)
                {
                    int index = 0;
                    foreach (var item in enumerable)
                    {
                        queryString.Add(ToQueryString(item, $"{propertyName}[{index}]"));
                        index++;
                    }
                }
                else
                {
                    queryString.Add(ToQueryString(value, propertyName));
                }
            }

            return string.Join("&", queryString);
        }
    }
}
