using T.Library.Model.Response;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using T.WebApi.Extensions;
using T.Library.Model;
using T.Library.Model.Interface;
using T.WebApi.Services.IRepositoryServices;
using T.Library.Model.Catalogs;
using T.WebApi.Services.UrlRecordServices;
using T.Library.Model.ViewsModel;

namespace T.WebApi.Services.CategoryServices
{
    public interface ICategoryService : ICategoryServiceCommon
    {
        Task<ServiceResponse<bool>> BulkUpdateProductCategoryAsync(List<ProductCategory> productCategories);
    }
    public class CategoryService : ICategoryService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<ProductCategory> _productCategoryRepository;
        private readonly IUrlRecordService _urlRecordService;

        public CategoryService(IMapper mapper, IRepository<Category> categoryRepository, IRepository<ProductCategory> productCategoryRepository, IUrlRecordService urlRecordService)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _productCategoryRepository = productCategoryRepository;
            _urlRecordService = urlRecordService;
        }

        public async Task<ServiceResponse<bool>> CreateCategoryAsync(CategoryModel model)
        {
            try
            {
                var category = _mapper.Map<Category>(model);

                category.CreatedOnUtc = DateTime.UtcNow;
                
                await _categoryRepository.CreateAsync(category);

                var seName = await _urlRecordService.ValidateSlug(category, model.SeName, category.Name, true);

                await _urlRecordService.SaveSlugAsync(category, seName);

                return new ServiceSuccessResponse<bool>();
            }
            catch (Exception ex)
            {
                return new ServiceErrorResponse<bool>(ex.Message);
            }
        }

        public async Task<ServiceResponse<bool>> UpdateCategoryAsync(CategoryModel model)
        {
            try
            {
                var category = _mapper.Map<Category>(model);
                category.UpdatedOnUtc = DateTime.UtcNow;
                await _categoryRepository.UpdateAsync(category);
                if (model.SeName != (await _urlRecordService.GetSeNameAsync(category)))
                {
                    model.SeName = await _urlRecordService.ValidateSlug(category, model.SeName, category.Name, true);

                    await _urlRecordService.SaveSlugAsync(category, model.SeName);
                }
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
                return new ServiceErrorResponse<bool>() { Message = ex.Message};
            }
        }

        public async Task<Category> GetCategoryByIdAsync(int categoryId)
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

        public async Task<Category> GetCategoryByNameAsync(string categoryName)
        {
            return await _categoryRepository.Table.Where(x => x.Deleted == false)
                .FirstOrDefaultAsync(x => x.Name == categoryName);
        }

        public async Task<List<ProductCategory>> GetProductCategoriesByCategoryIdAsync(int categoryId)
        {
            var productCategories = await _productCategoryRepository.Table.Where(x => x.CategoryId == categoryId).OrderByDescending(x=>x.DisplayOrder).ToListAsync();

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

        public async Task<ProductCategory> GetProductCategoryByIdAsync(int productCategoryId)
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
    }
}
