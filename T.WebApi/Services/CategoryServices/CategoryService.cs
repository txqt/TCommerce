using T.Library.Model.Response;
using Microsoft.EntityFrameworkCore;
using T.WebApi.Database.ConfigurationDatabase;
using AutoMapper;
using T.WebApi.Extensions;
using T.Library.Model.Common;

namespace T.WebApi.Services.CategoryServices
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllCategoryAsync();
        Task<ServiceResponse<Category>> GetCategoryByIdAsync(int categoryId);
        Task<ServiceResponse<bool>> CreateOrEditAsync(Category category);
        Task<ServiceResponse<bool>> DeleteCategoryByIdAsync(int id);
    }
    public class CategoryService : ICategoryService
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;

        public CategoryService(DatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<bool>> CreateOrEditAsync(Category category)
        {
            using (_context)
            {
                var categoryTable = await _context.Category.FirstOrDefaultAsync(x => x.Id == category.Id);
                if (categoryTable == null)
                {
                    category.CreatedOnUtc = DateTime.Now;
                    _context.Category.Add(category);
                }
                else
                {
                    if (_context.IsRecordUnchanged(categoryTable, category))
                    {
                        return new ServiceErrorResponse<bool>("Data is unchanged");
                    }
                    categoryTable.Name = category.Name;
                    categoryTable.Description = category.Description;
                    categoryTable.MetaKeywords = category.MetaKeywords;
                    categoryTable.MetaDescription = category.MetaDescription;
                    categoryTable.MetaTitle = category.MetaTitle;
                    categoryTable.ParentCategoryId = category.ParentCategoryId;
                    categoryTable.PictureId = category.PictureId;
                    categoryTable.ShowOnHomepage = category.ShowOnHomepage;
                    categoryTable.IncludeInTopMenu = category.IncludeInTopMenu;
                    categoryTable.Published = category.Published;
                    categoryTable.DisplayOrder = category.DisplayOrder;
                    categoryTable.UpdatedOnUtc = DateTime.UtcNow;
                    categoryTable.PriceRangeFiltering = category.PriceRangeFiltering;
                    categoryTable.PriceFrom = category.PriceFrom;
                    categoryTable.PriceTo = category.PriceTo;
                    categoryTable.ManuallyPriceRange = category.ManuallyPriceRange;
                }
                var result = await _context.SaveChangesAsync();
                if (result == 0)
                {
                    return new ServiceErrorResponse<bool>("Create category failed");
                }
                return new ServiceSuccessResponse<bool>();
            }
        }

        public async Task<ServiceResponse<bool>> DeleteCategoryByIdAsync(int id)
        {
            try
            {
                var category = await _context.Category.FirstOrDefaultAsync(x => x.Id == id);
                _context.Category.Remove(category);
                await _context.SaveChangesAsync();
                return new ServiceSuccessResponse<bool>();
            }
            catch (Exception ex)
            {
                return new ServiceErrorResponse<bool>(message: ex.Message);
            }
        }

        public async Task<ServiceResponse<Category>> GetCategoryByIdAsync(int categoryId)
        {
            using (_context)
            {
                var category = await _context.Category.FirstOrDefaultAsync(x => x.Id == categoryId);

                var response = new ServiceResponse<Category>
                {
                    Data = category,
                    Success = true
                };
                return response;
            }
        }

        public async Task<List<Category>> GetAllCategoryAsync()
        {
            using (_context)
            {
                return await _context.Category.ToListAsync();
            }
        }
    }
}
