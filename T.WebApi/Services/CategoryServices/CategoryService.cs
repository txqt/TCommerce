using T.Library.Model.Response;
using T.Library.Model;
using Microsoft.EntityFrameworkCore;
using T.WebApi.Database.ConfigurationDatabase;
using AutoMapper;

namespace T.WebApi.Services.CategoryServices
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllAsync();
        Task<ServiceResponse<ProductAttribute>> Get(int id);
        Task<ServiceResponse<bool>> CreateOrEditAsync(Category category);
        Task<ServiceResponse<bool>> DeleteAsync(int id);
    }
    public class CategoryService : ICategoryService
    {
        private readonly DatabaseContext _context;
        private readonly Mapper _mapper;

        public CategoryService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<bool>> CreateOrEditAsync(Category category)
        {
            using (_context)
            {
                var categoryTable = await _context.Category.FirstOrDefaultAsync(x=>x.Id == category.Id);
                if(categoryTable == null)
                {
                    _context.Category.Add(category);
                }
                else
                {
                    categoryTable = _mapper.Map<Category>(category);
                }
                var result = await _context.SaveChangesAsync();
                if (result == 0)
                {
                    return new ServiceErrorResponse<bool>("Create category failed");
                }
                return new ServiceSuccessResponse<bool>();
            }
        }

        public Task<ServiceResponse<bool>> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<ProductAttribute>> Get(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Category>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}
