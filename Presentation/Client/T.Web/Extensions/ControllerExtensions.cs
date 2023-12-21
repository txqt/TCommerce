using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using T.Web.Controllers;

namespace T.Web.Extensions
{
    public static class ControllerExtensions
    {
        public static IActionResult JsonWithPascalCase<T>(this BaseController controller, T data)
        {
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null,
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            return controller.Json(data, jsonSerializerOptions);
        }

        public static IActionResult JsonWithCustomOptions<T>(this BaseController controller, T data, JsonSerializerOptions options = null)
        {
            options ??= new JsonSerializerOptions();

            return controller.Json(data, options);
        }
    }
}
