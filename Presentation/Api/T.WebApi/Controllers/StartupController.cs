using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using T.Library.Model.Startup;
using T.WebApi.Services.DataSeederService;
using T.WebApi.Services.DbManageService;

namespace T.WebApi.Controllers
{
    [Route("api/startup")]
    [ApiController]
    public class StartupController : ControllerBase
    {
        private readonly DataSeeder _dataSeeder;
        private readonly IDbManageService _dbManageService;
        public StartupController(DataSeeder dataSeeder, IDbManageService dbManageService)
        {
            _dataSeeder = dataSeeder;
            _dbManageService = dbManageService;
        }

        [HttpPost("install")]
        public async Task<IActionResult> Install(StartupFormModel startupFormModel)
        {

            string connectionString = _dbManageService.BuildConnectionString(startupFormModel.ServerName, startupFormModel.DbName,
                startupFormModel.SqlUsername, startupFormModel.SqlPassword, startupFormModel.UseWindowsAuth);

            string connectionStringKey = "ConnectionStrings:DefaultConnection";

            if(AppSettingsExtensions.GetKey(connectionStringKey) is null)
            {
                AppSettingsExtensions.CreateKey(connectionStringKey);
            }
            AppSettingsExtensions.AddToKey(connectionStringKey, connectionString);

            if (startupFormModel.CreateDatabaseIfNotExist)
            {
                await _dbManageService.CreateDatabaseAsync();
            }

            if(startupFormModel.CreateSampleData)
            {
                await _dataSeeder.Initialize(sampleData: true);
            }

            return Ok();
        }
    }
}
