using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Diagnostics;
using T.WebApi.Database;

namespace T.WebApi.Services.DbManageService
{
    public interface IDbManageService
    {
        SqlConnectionStringBuilder GetConnectionStringBuilder();
        bool IsDatabaseInstalled();
        bool DatabaseExists();
        Task CreateDatabaseAsync(int triesToConnect = 10);
        string BuildConnectionString(string serverName, string dbName, string sqlUsername, string sqlPassword, bool useWindowsAuth);
        Task InitializeDatabase();
    }
    public class DbManageService : IDbManageService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        protected bool? _databaseIsInstalled;

        public DbManageService(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        public SqlConnectionStringBuilder GetConnectionStringBuilder()
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder(_configuration.GetConnectionString("DefaultConnection"));
            return connectionStringBuilder;
        }

        public bool IsDatabaseInstalled()
        {
            _databaseIsInstalled ??= !string.IsNullOrEmpty(GetConnectionStringBuilder()?.ConnectionString);

            return _databaseIsInstalled.Value;
        }

        public bool DatabaseExists()
        {
            try
            {
                using (var connection = new SqlConnection(GetConnectionStringBuilder().ConnectionString))
                {
                    connection.Open();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task CreateDatabaseAsync(int triesToConnect = 10)
        {
            if (DatabaseExists())
            {
                return;
            }

            var connectionStringBuilder = GetConnectionStringBuilder();
            var databaseName = connectionStringBuilder.InitialCatalog;

            // Create a new connection string without the database name
            connectionStringBuilder.InitialCatalog = "master";
            using (var connection = new SqlConnection(connectionStringBuilder.ConnectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"CREATE DATABASE [{databaseName}]";
                    await command.ExecuteNonQueryAsync();
                }
            }

            // Check if the database is ready
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    using (var context = new DatabaseContext())
                    {
                        if (DatabaseExists())
                        {
                            // Database is ready, you can start creating tables and seed data
                            return;
                        }
                    }
                }
                catch
                {
                    // Ignore any exception
                }

                // Wait for a second before trying again
                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            throw new Exception("Database is not ready after 10 seconds");
        }

        public string BuildConnectionString(string serverName, string dbName, string sqlUsername, string sqlPassword, bool useWindowsAuth)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = serverName,
                InitialCatalog = dbName,
                IntegratedSecurity = useWindowsAuth,
                PersistSecurityInfo = false,
                TrustServerCertificate = true
            };

            if (!useWindowsAuth)
            {
                builder.UserID = sqlUsername;
                builder.Password = sqlPassword;
            }

            return builder.ConnectionString;
        }

        public async Task InitializeDatabase()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

                await dbContext.Database.EnsureCreatedAsync();
            }
        }
        //public void UpdateEntities(params string[] sqlCommands)
        //{
        //    foreach (var sqlCommand in sqlCommands)
        //    {
        //        _context.Database.ExecuteSqlRaw(sqlCommand);
        //    }
        //}
    }
}
