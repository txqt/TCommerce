using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace T.WebApi.Attribute
{
    public class ValidationFilterAttribute : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            { 
                foreach (var key in context.ModelState.Keys)
                {
                    if (context.ModelState.TryGetValue(key, out var stateEntry) && stateEntry != null)
                    {
                        var errors = stateEntry.Errors;
                        foreach (var error in errors)
                        {
                            // Thực hiện xử lý với lỗi cụ thể ở đây
                            var errorMessage = error.ErrorMessage;
                            var exception = error.Exception;
                            // Đối với trường key, errorMessage chứa thông điệp lỗi và exception chứa thông tin về lỗi (nếu có)
                        }
                    }
                }
                context.Result = new UnprocessableEntityObjectResult(context.ModelState);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
