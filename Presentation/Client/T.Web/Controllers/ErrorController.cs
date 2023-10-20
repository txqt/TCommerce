using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using T.Web.Models;

namespace T.Web.Controllers
{
    public class ErrorController : BaseController
    {
        [Route("/Error/{statusCode}")]
        public IActionResult Error(int statusCode)
        {

            var errorViewModel = new ErrorViewModel();

            if(statusCode == 404)
            {
                errorViewModel.ErrorType = statusCode.ToString();
                errorViewModel.ErrorMessage = "Not Found";
            }
            else
            {
                errorViewModel.ErrorType = statusCode.ToString();
                errorViewModel.ErrorMessage = "Đã xảy ra lỗi";
            }

            return View("Error", errorViewModel);
        }
    }
}
