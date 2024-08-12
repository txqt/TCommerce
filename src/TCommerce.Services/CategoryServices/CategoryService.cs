using Microsoft.EntityFrameworkCore;
using AutoMapper;
using TCommerce.Services.IRepositoryServices;
using TCommerce.Services.UrlRecordServices;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Core.Models.Response;
using TCommerce.Core.Models.ViewsModel;
using TCommerce.Core.Models.Discounts;
using System.Drawing.Printing;
using TCommerce.Core.Interface;
using TCommerce.Core.Helpers;

namespace TCommerce.Services.CategoryServices
{
    public class CategoryService : ICategoryService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<ProductCategory> _productCategoryRepository;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IRepository<DiscountCategoryMapping> _discountCategoryMappingRepository;

        public CategoryService(IMapper mapper, IRepository<Category> categoryRepository, IRepository<ProductCategory> productCategoryRepository, IUrlRecordService urlRecordService, IRepository<DiscountCategoryMapping> discountCategoryMappingRepository)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _productCategoryRepository = productCategoryRepository;
            _urlRecordService = urlRecordService;
            _discountCategoryMappingRepository = discountCategoryMappingRepository;
        }

        public async Task<ServiceResponse<bool>> CreateCategoryAsync(Category category)
        {
            try
            {

                category.CreatedOnUtc = DateTime.UtcNow;

                await _categoryRepository.CreateAsync(category);

                return new ServiceSuccessResponse<bool>();
            }
            catch (Exception ex)
            {
                return new ServiceErrorResponse<bool>(ex.Message);
            }
        }

        public async Task<ServiceResponse<bool>> UpdateCategoryAsync(Category category)
        {
            try
            {
                category.UpdatedOnUtc = DateTime.UtcNow;

                await _categoryRepository.UpdateAsync(category);

                return new ServiceSuccessResponse<bool>();
            }
            catch (Exception ex)
            {
                return new ServiceErrorResponse<bool>(ex.Message);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteCategoryByIdAsync(int id)
        {
            try
            {
                await _categoryRepository.DeleteAsync(id);
                return new ServiceSuccessResponse<bool>();
            }
            catch (Exception ex)
            {
                return new ServiceErrorResponse<bool>() { Message = ex.Message };
            }
        }

        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            return await _categoryRepository.Table.Where(x => x.Deleted == false)
                .FirstOrDefaultAsync(x => x.Id == categoryId);
        }

        public async Task<List<Category>> GetAllCategoryAsync()
        {
            return (await _categoryRepository.GetAllAsync(x =>
            {
                return from c in x
                       where c.Deleted == false
                       select c;
            })).ToList();
        }

        public async Task<Category?> GetCategoryByNameAsync(string categoryName)
        {
            return await _categoryRepository.Table.Where(x => x.Deleted == false)
                .FirstOrDefaultAsync(x => x.Name == categoryName);
        }

        public async Task<List<ProductCategory>> GetProductCategoriesByCategoryIdAsync(int categoryId)
        {
            var productCategories = await _productCategoryRepository.Table.Where(x => x.CategoryId == categoryId).OrderByDescending(x => x.DisplayOrder).ToListAsync();

            return productCategories;
        }

        public async Task<ServiceResponse<bool>> CreateProductCategoryAsync(ProductCategory productCategory)
        {
            try
            {
                await _productCategoryRepository.CreateAsync(productCategory);
                return new ServiceSuccessResponse<bool>();
            }
            catch (Exception ex)
            {
                return new ServiceErrorResponse<bool>() { Message = ex.Message };
            }
        }

        public async Task<ServiceResponse<bool>> BulkCreateProductCategoriesAsync(List<ProductCategory> productCategories)
        {
            try
            {
                await _productCategoryRepository.BulkCreateAsync(productCategories);
                return new ServiceSuccessResponse<bool>();
            }
            catch (Exception ex)
            {
                return new ServiceErrorResponse<bool>(ex.Message);
            }
        }

        public async Task<ServiceResponse<bool>> UpdateProductCategoryAsync(ProductCategory productCategory)
        {
            try
            {
                await _productCategoryRepository.UpdateAsync(productCategory);
            }
            catch (Exception ex)
            {
                return new ServiceErrorResponse<bool>(ex.Message);
            }

            return new ServiceSuccessResponse<bool>();
        }

        public async Task<ServiceResponse<bool>> DeleteProductCategoryMappingById(int productCategoryId)
        {
            try
            {
                await _productCategoryRepository.DeleteAsync(productCategoryId);
                return new ServiceSuccessResponse<bool>();
            }
            catch (Exception ex)
            {
                return new ServiceErrorResponse<bool>() { Message = ex.Message };
            }
        }

        public async Task<ProductCategory?> GetProductCategoryByIdAsync(int productCategoryId)
        {
            return await _productCategoryRepository.Table
                .FirstOrDefaultAsync(x => x.Id == productCategoryId);
        }

        public async Task<List<ProductCategory>> GetProductCategoriesByProductIdAsync(int productId)
        {
            return await _productCategoryRepository.Table.Where(x => x.ProductId == productId)
                .ToListAsync();
        }

        public Task<ServiceResponse<bool>> BulkUpdateProductCategoryAsync(List<ProductCategory> productCategories)
        {
            throw new NotImplementedException();
        }

        public async Task ClearDiscountCategoryMappingAsync(Discount discount)
        {
            ArgumentNullException.ThrowIfNull(discount);

            var mappings = _discountCategoryMappingRepository.Table.Where(dcm => dcm.DiscountId == discount.Id);

            foreach (var pdm in await mappings.ToListAsync())
            {
                await _discountCategoryMappingRepository.DeleteAsync(pdm.Id);
            }
        }

        public Task<List<DiscountCategoryMapping>> GetAllDiscountsAppliedToCategoryAsync(int categoryId)
        {
            throw new NotImplementedException();
        }

        public async Task<DiscountCategoryMapping> GetDiscountAppliedToCategoryAsync(int categoryId, int discountId)
        {
            return await _discountCategoryMappingRepository.Table
            .FirstOrDefaultAsync(dcm => dcm.EntityId == categoryId && dcm.DiscountId == discountId);
        }

        public async Task CreateDiscountCategoryMappingAsync(DiscountCategoryMapping discountCategoryMapping)
        {
            await _discountCategoryMappingRepository.CreateAsync(discountCategoryMapping);
        }

        public async Task DeleteDiscountCategoryMappingAsync(int discountCategoryMappingId)
        {
            await _discountCategoryMappingRepository.DeleteAsync(discountCategoryMappingId);
        }

        public async Task<List<Category>> GetCategoriesByAppliedDiscountAsync(int? discountId = null, bool showHidden = false)
        {
            var categories = _categoryRepository.Query;

            if (discountId.HasValue)
                categories = from category in categories
                             join dcm in _discountCategoryMappingRepository.Table on category.Id equals dcm.EntityId
                             where dcm.DiscountId == discountId.Value
                             select category;

            if (!showHidden)
                categories = categories.Where(category => !category.Deleted);

            categories = categories.OrderBy(category => category.DisplayOrder).ThenBy(category => category.Id);

            return await categories.ToListAsync();
        }

        public async Task<List<Category>?> GetCategoriesByIdsAsync(List<int> ids)
        {
            return (await _categoryRepository.GetByIdsAsync(ids)).ToList();
        }

        public async Task<List<Category>> GetAllCategoriesDisplayedOnHomepageAsync(bool showHidden = false)
        {
            var categories = await _categoryRepository.GetAllAsync(query =>
            {
                return from c in query
                       orderby c.DisplayOrder, c.Id
                       where c.Published &&
                             (showHidden ? c.Deleted : !c.Deleted) &&
                             c.ShowOnHomepage
                       select c;
            }, CacheKeysDefault<Category>.AllPrefix + "displayed.on.home.page");

            return categories.ToList();
        }
    }
}
