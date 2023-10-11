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

        public CategoryService(IMapper mapper, IRepository<Category> categoryRepository)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        public async Task<ServiceResponse<bool>> CreateCategoryAsync(Category category)
        {
            try
            {
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
    }
}
