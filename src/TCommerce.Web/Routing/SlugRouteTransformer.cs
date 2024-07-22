using Microsoft.AspNetCore.Mvc.Routing;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Core.Models.Seo;

namespace TCommerce.Web.Routing
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
                if (await _urlRecordService.GetBySlugAsync(slug.ToString()) is not UrlRecord urlRecord)
                    return values;

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
                            controllerName = "Catalog";
                            actionName = "Category";
                            break;

                        case nameof(Manufacturer):
                            controllerName = "Catalog";
                            actionName = "Manufacturer";
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
