﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using T.Library.Model.Enum;
using T.Web.Areas.Attribute;
using T.Web.Areas.Services.Database;
using T.Web.Controllers;

namespace T.Web.Areas.Database.Controllers
{
    [Area("Database")]
    [Route("/database-manage/[action]")]
    [Authorize]
    [CustomAuthorizationFilter(RoleName.Admin)]
    public class DBManageController : BaseController
    {
        private readonly IDatabaseControl _databaseControl;

        public DBManageController(IDatabaseControl databaseControl)
        {
            _databaseControl = databaseControl;
        }


        public async Task<IActionResult> Index()
        {
            var databaseResponse = await _databaseControl.GetDbInfo();
            return View(databaseResponse);
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
