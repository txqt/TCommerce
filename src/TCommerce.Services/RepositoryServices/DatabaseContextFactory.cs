using Microsoft.EntityFrameworkCore;
using TCommerce.Data;

namespace TCommerce.Services.IRepositoryServices
{
    public class ApplicationDbContextFactory
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public ApplicationDbContextFactory(DbContextOptions<ApplicationDbContext> options)
        {
            _options = options;
        }

        public ApplicationDbContext CreateDbContext()
        {
            return new ApplicationDbContext(_options);
        }
    }
}
