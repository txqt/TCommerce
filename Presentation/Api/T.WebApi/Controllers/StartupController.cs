﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using T.Library.Model.Roles.RoleName;
using T.Library.Model.Startup;
using T.Library.Model.Users;
using T.WebApi.Attribute;
using T.WebApi.Services.DataSeederService;
using T.WebApi.Services.DbManageService;

namespace T.WebApi.Controllers
{
    [Route("api/startup")]
    [ApiController]
    public class StartupController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;
        public StartupController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [HttpPost("install")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Install([FromBody]StartupFormModel model)
        {
            var _dbManageService = _serviceProvider.GetRequiredService<IDbManageService>();
            string connectionString = _dbManageService.BuildConnectionString(model.ServerName, model.DbName,
                model.SqlUsername, model.SqlPassword, model.UseWindowsAuth);

            string connectionStringKey = "ConnectionStrings:DefaultConnection";

            if (AppSettingsExtensions.GetKey(connectionStringKey) is null)
            {
                AppSettingsExtensions.CreateKey(connectionStringKey);
                throw new Exception("Some resources were lost but may have been fixed, please try again");
            }
            AppSettingsExtensions.AddToKey(connectionStringKey, connectionString);

            if (_dbManageService.DatabaseExists())
            {
                return Ok("Database already exists");
            }
            _dbManageService = _serviceProvider.GetRequiredService<IDbManageService>();
            var _dataSeeder = _serviceProvider.GetRequiredService<DataSeeder>();
            if (model.CreateDatabaseIfNotExist)
            {
                await _dbManageService.CreateDatabaseAsync();
            }

            await _dbManageService.InitializeDatabase();

            await _dataSeeder.Initialize(model.AdminEmail, model.AdminPassword, model.CreateSampleData);

            return Ok();
        }
    }
}