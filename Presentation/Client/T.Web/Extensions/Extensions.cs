using System.Net;
using System.Text;

namespace T.Web.Extensions
{
    public static class Extensions
    {
        public static string ToQueryParameters<T>(this T item, Func<T, object> selector)
        {
            if (item == null || selector == null)
                throw new ArgumentNullException();

            StringBuilder queryBuilder = new StringBuilder();

            var properties = selector(item).GetType().GetProperties();
            foreach (var prop in properties)
            {
                var value = prop.GetValue(selector(item));
                if (value != null)
                {
                    string encodedName = WebUtility.UrlEncode(prop.Name.ToLower());
                    string encodedValue = WebUtility.UrlEncode(value.ToString());
                    queryBuilder.Append($"{encodedName}={encodedValue}&");
                }
            }

            // remove the trailing '&'
            if (queryBuilder.Length > 0)
                queryBuilder.Length--;

            return queryBuilder.ToString();
        }
    }
}
