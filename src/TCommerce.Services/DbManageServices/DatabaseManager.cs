using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TCommerce.Data;

namespace TCommerce.Services.DbManageServices
{
    public class DatabaseManager
    {
        protected static bool? _databaseIsInstalled;
        private IServiceProvider _serviceProvider;

        public DatabaseManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static SqlConnectionStringBuilder GetConnectionStringBuilder(string? connectionStringKey = null)
        {
            if (string.IsNullOrEmpty(connectionStringKey))
            {
                connectionStringKey = "ConnectionStrings:DefaultConnection";
            }
            var connectionStringBuilder = new SqlConnectionStringBuilder(AppSettingsExtensions.GetKey(connectionStringKey));
            return connectionStringBuilder;
        }

        public static bool IsDatabaseInstalled()
        {
            _databaseIsInstalled ??= !string.IsNullOrEmpty(GetConnectionStringBuilder()?.ConnectionString);

            return _databaseIsInstalled.Value;
        }

        public static bool DatabaseExists()
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

        public static async Task CreateDatabaseAsync(int triesToConnect = 10)
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
                    using (var context = new ApplicationDbContext())
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

        public static string BuildConnectionString(string serverName, string dbName, string sqlUsername, string sqlPassword, bool useWindowsAuth)
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

        public async Task InitializeTables()
        {
            var services = new ServiceCollection();
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(AppSettingsExtensions.GetKey("ConnectionStrings:DefaultConnection"));
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            _serviceProvider = services.BuildServiceProvider();
            var dbContext = _serviceProvider.GetRequiredService<ApplicationDbContext>();

            await dbContext.Database.EnsureCreatedAsync();

        }
    }
}
