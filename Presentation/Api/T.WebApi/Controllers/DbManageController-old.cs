//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using T.Library.Model;
//using T.WebApi.Database.ConfigurationDatabase;
//using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
//using T.WebApi.Attribute;
//using Microsoft.Extensions.Caching.Distributed;
//using T.WebApi.Services.CacheServices;
//using System.Data;
//using Azure.Core;
//using T.Library.Model.Users;
//using System.Security.Claims;
//using Microsoft.AspNetCore.Identity;
//using T.WebApi.Extensions;
//using T.Library.Model.Roles.RoleName;
//using T.Library.Model.Security;

//namespace T.WebApi.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    [CustomAuthorizationFilter(RoleName.Admin)]
//    public class DbManageController : ControllerBase
//    {
//        private readonly DatabaseContext _databaseContext;
//        private readonly UserManager<User> _userManager;
//        private readonly RoleManager<Role> roleManager;

//        public DbManageController(DatabaseContext databaseContext, RoleManager<Role> roleManager, UserManager<User> userManager)
//        {
//            this._databaseContext = databaseContext;
//            this.roleManager = roleManager;
//            _userManager = userManager;
//        }
//        [HttpGet]
//        public ActionResult Index()
//        {

//            var connect = _databaseContext.Database.GetDbConnection();
//            var all_tables = connect.State == ConnectionState.Open
//                ? _databaseContext.Model.GetEntityTypes().Select(t => t.GetTableName()).ToList()
//                : new List<string?>();

//            var dbResponse = new DatabaseControlResponse
//            {
//                dbname = connect.Database,
//                source = connect.DataSource,
//                state = (int)connect.State,
//                can_connect = _databaseContext.Database.CanConnect(),
//                list_applied_migration = _databaseContext.Database.GetAppliedMigrations().ToList(),
//                list_migration_pending = _databaseContext.Database.GetPendingMigrations().ToList(),
//                list_tables = all_tables
//            };

//            return Ok(dbResponse);
//        }

//        [HttpDelete]
//        public IActionResult DeleteDatabase()
//        {
//            var result = _databaseContext.Database.EnsureDeleted();
//            if (result)
//            {
//                return Ok("Đã xoá cơ sở dữ liệu thành công!");
//            }
//            return BadRequest("Xoá cơ sở dữ liệu không thành công!");
//        }

//        [HttpPost("Migrate")]
//        public async Task<IActionResult> Migrate()
//        {
//            await _databaseContext.Database.MigrateAsync();
//            return Ok("Cập nhật database thành công!");
//        }
//        [HttpGet("Seed-Data")]
//        public async Task<IActionResult> Seed()
//        {
//            var rolenames = typeof(RoleName).GetFields().ToList();
//            foreach (var item in rolenames)
//            {
//                string? name = item.GetRawConstantValue().ToString();
//                var ffound = await roleManager.FindByNameAsync(name);
//                if (ffound == null)
//                {
//                    await roleManager.CreateAsync(new Role(name));
//                }
//            }

//            var user2 = await _userManager.FindByEmailAsync("hovanthanh12102002@gmail.com");
//            if (user2 == null)
//            {
//                user2 = new User()
//                {
//                    Id = Guid.NewGuid(),

//                    FirstName = "Văn Thành",
//                    LastName = "Hồ",
//                    Email = "hovanthanh12102002@gmail.com",
//                    NormalizedEmail = "hovanthanh12102002@gmail.com",
//                    PhoneNumber = "032232131",
//                    UserName = "thanhhv",
//                    NormalizedUserName = "THANHHV",
//                    CreatedDate = AppExtensions.GetDateTimeNow(),
//                    EmailConfirmed = true // không cần xác thực email nữa , 
//                };
//                await _userManager.CreateAsync(user2, "123321");
//                await _userManager.AddToRoleAsync(user2, RoleName.Admin);
//                return Ok();
//            }

//            var user3 = await _userManager.FindByEmailAsync("hovanthanh@gmail.com");
//            if (user3 == null)
//            {
//                user3 = new User()
//                {
//                    Id = Guid.NewGuid(),

//                    FirstName = "Văn Thành",
//                    LastName = "Hồ",
//                    Email = "hovanthanh@gmail.com",
//                    NormalizedEmail = "hovanthanh@gmail.com",
//                    PhoneNumber = "032232131",
//                    UserName = "thanhhv2",
//                    NormalizedUserName = "THANHHV2",
//                    CreatedDate = AppExtensions.GetDateTimeNow(),
//                    EmailConfirmed = true // không cần xác thực email nữa , 
//                };
//                await _userManager.CreateAsync(user3, "123321");
//                await _userManager.AddToRoleAsync(user3, RoleName.Customer);
//                return Ok();
//            }
//            return BadRequest();
//        }
//    }
//}
