using Microsoft.EntityFrameworkCore;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Data;

namespace TCommerce.Services.HomePageServices
{
    public interface IHomePageService
    {
        Task<List<Category>> ShowCategoriesOnHomePage();
    }
    public class HomePageService : IHomePageService
    {
        private readonly ApplicationDbContext _context;

        public HomePageService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> ShowCategoriesOnHomePage()
        {
            return await _context.Categories.Where(x=>x.IncludeInTopMenu && x.Published).ToListAsync();
        }
    }
}
