using T.Library.Model.Response;
using Microsoft.EntityFrameworkCore;
using T.WebApi.Database.ConfigurationDatabase;
using AutoMapper;
using T.WebApi.Extensions;
using T.Library.Model.Common;
using T.Library.Model;
using T.Library.Model.Interface;
using Microsoft.AspNetCore.Mvc;
using T.WebApi.Services.IRepositoryServices;

namespace T.WebApi.Services.CategoryServices
{
    public class CategoryService : ICategoryService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<ProductCategory> _productCategoryRepository;

        public CategoryService(IMapper mapper, IRepository<Category> categoryRepository, IRepository<ProductCategory> productCategoryRepository)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _productCategoryRepository = productCategoryRepository;
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
                return new ServiceErrorResponse<bool>() { Message = ex.Message};
            }
        }

        public async Task<ServiceResponse<Category>> GetCategoryByIdAsync(int categoryId)
        {
            var category = await _categoryRepository.Table.Where(x => x.Deleted == false)
                .FirstOrDefaultAsync(x => x.Id == categoryId);

            var response = new ServiceResponse<Category>
            {
                Data = category,
                Success = true
            };
            return response;
        }

        public async Task<List<Category>> GetAllCategoryAsync()
        {
            return (await _categoryRepository.GetAllAsync()).ToList();
        }

        public async Task<ServiceResponse<Category>> GetCategoryByNameAsync(string categoryName)
        {
            var category = await _categoryRepository.Table.Where(x => x.Deleted == false)
                .FirstOrDefaultAsync(x => x.Name == categoryName);

            var response = new ServiceResponse<Category>
            {
                Data = category,
                Success = true
            };
            return response;
        }

        public async Task<List<ProductCategory>> GetProductCategoriesByCategoryIdAsync(int categoryId)
        {
            var productCategories = await _productCategoryRepository.Table.Where(x => x.CategoryId == categoryId).ToListAsync();

            return productCategories;
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

        public async Task<ServiceResponse<bool>> DeleteCategoryMappingById(int productCategoryId)
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
    }
}
