using Microsoft.AspNetCore.Mvc.Routing;
using T.Library.Model;
using T.Library.Model.Catalogs;
using T.Web.Services.UrlRecordService;

namespace T.Web.Routing
{
    public class SlugRouteTransformer : DynamicRouteValueTransformer
    {
        private readonly IUrlRecordService _urlRecordService;

        public SlugRouteTransformer(IUrlRecordService urlRecordService)
        {
            _urlRecordService = urlRecordService;
        }
        public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
        {
            // Kiểm tra xem giá trị "slug" có tồn tại không
            if (values.TryGetValue("slug", out var slugValue) && slugValue is string slug)
            {
                // Lấy thông tin từ bảng UrlRecord
                var urlRecord = await _urlRecordService.GetBySlugAsync(slug);

                if (urlRecord != null)
                {
                    // Xác định controller và action dựa trên EntityName sử dụng switch
                    string controllerName;
                    string actionName;

                    switch (urlRecord.EntityName)
                    {
                        case nameof(Product):
                            controllerName = "Product";
                            actionName = "Details";
                            break;

                        case nameof(Category):
                            controllerName = "Categories";
                            actionName = "Details";
                            break;

                        default:
                            controllerName = "Home";
                            actionName = "NotFound";
                            break;
                    }

                    // Thêm giá trị vào RouteValueDictionary
                    return new RouteValueDictionary
                    {
                        ["controller"] = controllerName,
                        ["action"] = actionName,
                        ["id"] = urlRecord.EntityId,
                        ["slug"] = slug
                    };
                }
            }

            // Nếu không tìm thấy thông tin, có thể chuyển hướng hoặc xử lý lỗi tùy thuộc vào yêu cầu của bạn
            httpContext.Response.StatusCode = 404;
            return null;
        }
    }
}
