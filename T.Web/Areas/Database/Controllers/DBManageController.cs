using Microsoft.AspNetCore.Mvc;
using T.Web.Areas.Services.Database;

namespace T.Web.Areas.Database.Controllers
{
    [Area("Database")]
    [Route("/database-manage/[action]")]
    public class DBManageController : Controller
    {
        private readonly IDatabaseControl _databaseControl;

        public DBManageController(IDatabaseControl databaseControl)
        {
            this._databaseControl = databaseControl;
        }

        
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult DeleteDb()
        {

            return View();
        }
        [TempData]
        public string StatusMessage { get; set; }
        [HttpPost]
        public async Task<IActionResult> DeleteDbAsync()
        {
            var success = await _databaseControl.DeleteDb();
            StatusMessage = success ? "Xoá database thành công !" : "Không xoá được !";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Migrate()
        {
            var success = await _databaseControl.MigrateDb();
            StatusMessage = success ? "Cập nhật database thành công !" : "Cập nhật database thất bại !";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> SeedData()
        {
            var success = await _databaseControl.SeedData();
            StatusMessage = success ? "Seed data thành công !" : "Seed data thất bại !";
            return RedirectToAction(nameof(Index));
        }
    }
}
