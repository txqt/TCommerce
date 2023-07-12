using Microsoft.AspNetCore.Mvc;

namespace T.Web.Areas.Admin.Models
{
    public class test : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Thực hiện các logic của thành phần ở đây
            // Ví dụ: Lấy danh sách sản phẩm từ cơ sở dữ liệu

            return View();
        }
    }
}
