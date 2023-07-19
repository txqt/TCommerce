using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using T.Web.Models;

namespace T.Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("/Error/{statusCode}")]
        public IActionResult Error(int statusCode)
        {
            var feature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            var errorViewModel = new ErrorViewModel
            {
                ErrorType = "Lỗi không xác định",
                ErrorMessage = "Đã xảy ra lỗi khi xử lý yêu cầu của bạn."
            };

            // Kiểm tra nếu có lỗi xảy ra
            if (feature != null)
            {
                errorViewModel.ErrorType = statusCode.ToString();
                errorViewModel.ErrorMessage = "Đã xảy ra lỗi khi xử lý yêu cầu của bạn.";
            }

            return View("Error", errorViewModel);
        }
    }
}
