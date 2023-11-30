using Microsoft.EntityFrameworkCore;
using T.Library.Model.Catalogs;
using T.WebApi.Database;

namespace T.WebApi.Services.HomePageServices
{
    public interface IHomePageService
    {
        Task<List<Category>> ShowCategoriesOnHomePage();
    }
    public class HomePageService : IHomePageService
    {
        private readonly DatabaseContext _context;

        public HomePageService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> ShowCategoriesOnHomePage()
        {
            return await _context.Category.Where(x=>x.IncludeInTopMenu && x.Published).ToListAsync();
        }
    }
}
