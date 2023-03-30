using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using T.Library.Model;
using T.WebApi.Database.ConfigurationDatabase;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using T.WebApi.Attribute;
using T.Library.Model.Enum;
using Microsoft.Extensions.Caching.Distributed;

namespace T.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DbManageController : ControllerBase
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IDistributedCache _cache;

        public DbManageController(DatabaseContext databaseContext, IDistributedCache cache)
        {
            this._databaseContext = databaseContext;
            _cache = cache;
        }
        [HttpGet]
        [CustomAuthorizationFilter(RoleName.Admin)]
        public ActionResult Index()
        {
            var connect = _databaseContext.Database.GetDbConnection();
            var dbname = connect.Database;
            var can_connect = _databaseContext.Database.CanConnect();
            var all_tables = new List<string?>();

            var list_applied_migraiton = _databaseContext.Database.GetAppliedMigrations().ToList();
            var list_migration_pending = _databaseContext.Database.GetPendingMigrations().ToList();

            if (can_connect)
            {
                all_tables = _databaseContext.Model.GetEntityTypes().Select(t => t.GetTableName()).ToList();
            }
            return Ok(new DatabaseControlResponse
            {
                dbname = dbname,
                source = connect.DataSource,
                state = (int)connect.State,
                can_connect = can_connect,
                list_applied_migration = list_applied_migraiton,
                list_migration_pending = list_migration_pending,
                list_tables = all_tables
            });
        }

        [HttpDelete]
        [CustomAuthorizationFilter(RoleName.Admin)]
        public IActionResult DeleteDatabase()
        {
            var result = _databaseContext.Database.EnsureDeleted();
            if (result)
            {
                return Ok("Đã xoá cơ sở dữ liệu thành công!");
            }
            return BadRequest("Xoá cơ sở dữ liệu không thành công!");
        }

        [HttpPost("Migrate")]
        public async Task<IActionResult> Migrate()
        {
            await _databaseContext.Database.MigrateAsync();
            return Ok("Cập nhật database thành công!");
        }
    }
}
