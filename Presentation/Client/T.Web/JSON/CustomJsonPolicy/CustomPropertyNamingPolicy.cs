using System.Text.Json;

namespace T.Web.JSON.CustomJsonPolicy
{
    public class CustomPropertyNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return name;
        }
    }
}
