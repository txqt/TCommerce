using Microsoft.EntityFrameworkCore;
using T.WebApi.Database;

namespace T.WebApi.Services.IRepositoryServices
{
    public class DatabaseContextFactory
    {
        private readonly DbContextOptions<DatabaseContext> _options;

        public DatabaseContextFactory(DbContextOptions<DatabaseContext> options)
        {
            _options = options;
        }

        public DatabaseContext CreateDbContext()
        {
            return new DatabaseContext(_options);
        }
    }
}
