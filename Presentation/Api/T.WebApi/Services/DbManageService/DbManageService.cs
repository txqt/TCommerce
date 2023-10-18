using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using T.WebApi.Database.ConfigurationDatabase;

namespace T.WebApi.Services.DbManageService
{
    public interface IDbManageService
    {
        SqlConnectionStringBuilder GetConnectionStringBuilder();
        bool DatabaseExists();
        Task CreateDatabaseAsync(int triesToConnect = 10);
        string BuildConnectionString(string serverName, string dbName, string sqlUsername, string sqlPassword, bool useWindowsAuth);
    }
    public class DbManageService : IDbManageService
    {
        private readonly IConfiguration _configuration;
        private readonly DatabaseContext _context;

        public DbManageService(IConfiguration configuration, DatabaseContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public SqlConnectionStringBuilder GetConnectionStringBuilder()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            var builder = new SqlConnectionStringBuilder(connectionString);
            return builder;
        }

        public bool DatabaseExists()
        {
            return _context.Database.CanConnect();
        }

        public async Task CreateDatabaseAsync(int triesToConnect = 10)
        {
            var connectionStringBuilder = GetConnectionStringBuilder();
            var databaseName = connectionStringBuilder.InitialCatalog;

            // Create a new connection string without the database name
            connectionStringBuilder.InitialCatalog = "";
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
                        if (context.Database.CanConnect())
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
                IntegratedSecurity = useWindowsAuth
            };

            if (!useWindowsAuth)
            {
                builder.UserID = sqlUsername;
                builder.Password = sqlPassword;
            }

            return builder.ConnectionString;
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
