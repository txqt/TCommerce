using AutoMapper;
using Microsoft.EntityFrameworkCore;
using T.Library.Model;
using T.Library.Model.Interface;
using T.Library.Model.Response;
using T.WebApi.Database.ConfigurationDatabase;
using T.WebApi.Extensions;
using T.WebApi.Services.IRepositoryServices;

namespace T.WebApi.Services.ProductServices
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IRepository<ProductCategory> _productCategoryRepository;

        public ProductCategoryService(IRepository<ProductCategory> productCategoryRepository)
        {
            _productCategoryRepository = productCategoryRepository;
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

        public async Task<ServiceResponse<bool>> DeleteProductCategoryAsync(int productCategoryId)
        {
            try
            {
                await _productCategoryRepository.DeleteAsync(productCategoryId);
                return new ServiceSuccessResponse<bool>();
            }
            catch (Exception ex)
            {
                return new ServiceErrorResponse<bool>(ex.Message);
            }
        }

        public async Task<ProductCategory> GetProductCategoryById(int productCategoryId)
        {
            var productCategory = await _productCategoryRepository.Table
                .FirstOrDefaultAsync(x => x.Id == productCategoryId);

            return productCategory;
        }

        //public async Task<List<ProductCategory>> GetAllProductCategoryAsync()
        //{
        //    return (await _productCategoryRepository.GetAllAsync()).ToList();
        //}

        public async Task<List<ProductCategory>> GetProductCategoriesByProductId(int productId)
        {
            var productCategoryList = await _productCategoryRepository.Table.Where(x => x.ProductId == productId)
               .ToListAsync();

            return productCategoryList;
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
    }
}
