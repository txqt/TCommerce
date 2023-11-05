using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using T.Library.Model;
using T.Library.Model.Interface;
using T.Library.Model.Response;
using T.WebApi.Database.ConfigurationDatabase;
using T.WebApi.Extensions;
using T.WebApi.Services.IRepositoryServices;

namespace T.WebApi.Services.ProductServices
{
    public interface IProductAttributeService : IProductAttributeCommon
    {
        Task<PagedList<ProductAttribute>> GetAllPagedAsync(ProductAttributeParameters productAttributeParameters);
    }
    public class ProductAttributeService : IProductAttributeService
    {
        private readonly IRepository<ProductAttribute> _productAttributeRepository;
        private readonly IRepository<ProductAttributeMapping> _productAttributeMappingRepository;
        private readonly IRepository<ProductAttributeValue> _productAttributeValueRepository;
        public ProductAttributeService(IRepository<ProductAttribute> productAttributeRepository, IRepository<ProductAttributeMapping> productAttributeMappingRepository, IRepository<ProductAttributeValue> productAttributeValueRepository)
        {
            _productAttributeRepository = productAttributeRepository;
            _productAttributeMappingRepository = productAttributeMappingRepository;
            _productAttributeValueRepository = productAttributeValueRepository;
        }

        public async Task<PagedList<ProductAttribute>> GetAllPagedAsync(ProductAttributeParameters productAttributeParameters)
        {
            var list_product_attribute = new List<ProductAttribute>();

            list_product_attribute = await _productAttributeRepository.Table
                .SearchByString(productAttributeParameters.SearchText)
                .Sort(productAttributeParameters.OrderBy)
                .ToListAsync();

            return PagedList<ProductAttribute>
                        .ToPagedList(list_product_attribute, productAttributeParameters.PageNumber, productAttributeParameters.PageSize);
        }

        public async Task<List<ProductAttribute>> GetAllProductAttributeAsync()
        {
            return (await _productAttributeRepository.GetAllAsync()).ToList();
        }

        public async Task<ServiceResponse<ProductAttribute>> GetProductAttributeByIdAsync(int id)
        {
            var response = new ServiceResponse<ProductAttribute>
            {
                Data = await _productAttributeRepository.GetByIdAsync(id),
                Success = true
            };
            return response;
        }

        public async Task<ServiceResponse<ProductAttribute>> GetProductAttributeByName(string name)
        {
            var response = new ServiceResponse<ProductAttribute>
            {
                Data = await _productAttributeRepository.Table
                    .FirstOrDefaultAsync(x => x.Name == name),
                Success = true
            };
            return response;
        }

        public async Task<ServiceResponse<bool>> CreateProductAttributeAsync(ProductAttribute productAttribute)
        {
            try
            {
                await _productAttributeRepository.CreateAsync(productAttribute);
                return new ServiceSuccessResponse<bool>();
            }
            catch(Exception ex)
            {
                return new ServiceErrorResponse<bool>(ex.Message);
            }
        }

        public async Task<ServiceResponse<bool>> UpdateProductAttributeAsync(ProductAttribute productAttribute)
        {
            try
            {
                await _productAttributeRepository.UpdateAsync(productAttribute);
                return new ServiceSuccessResponse<bool>();
            }
            catch (Exception ex)
            {
                return new ServiceErrorResponse<bool>(ex.Message);
            }

        }

        public async Task<ServiceResponse<bool>> DeleteProductAttributeByIdAsync(int id)
        {
            try
            {
                await _productAttributeRepository.DeleteAsync(id);
                return new ServiceSuccessResponse<bool>();
            }
            catch (Exception ex)
            {
                return new ServiceErrorResponse<bool>() { Message = ex.Message };
            }
        }

        public async Task<ServiceResponse<ProductAttributeMapping>> GetProductAttributeMappingByIdAsync(int id)
        {
            var productAttributeMapping = await _productAttributeMappingRepository.GetByIdAsync(id);

            var response = new ServiceResponse<ProductAttributeMapping>
            {
                Data = productAttributeMapping,
                Success = true
            };
            return response;
        }

        public async Task<ServiceResponse<List<ProductAttributeMapping>>> GetProductAttributesMappingByProductIdAsync(int id)
        {
            var productAttributeMapping = await _productAttributeMappingRepository.Table.Where(x => x.ProductId == id)
                .Include(x => x.ProductAttribute)
                .ToListAsync();

            var response = new ServiceResponse<List<ProductAttributeMapping>>
            {
                Data = productAttributeMapping,
                Success = true
            };
            return response;
        }

        public async Task<ServiceResponse<bool>> CreateProductAttributeMappingAsync(ProductAttributeMapping productAttributeMapping)
        {
            try
            {
                await _productAttributeMappingRepository.CreateAsync(productAttributeMapping);
                return new ServiceSuccessResponse<bool>();
            }catch (Exception ex)
            {
                return new ServiceErrorResponse<bool>() { Message = ex.Message};
            }
        }

        public async Task<ServiceResponse<bool>> UpdateProductAttributeMappingAsync(ProductAttributeMapping productAttributeMapping)
        {
            try
            {
                await _productAttributeMappingRepository.UpdateAsync(productAttributeMapping);
            }
            catch (Exception ex)
            {
                return new ServiceErrorResponse<bool>(ex.Message);
            }

            return new ServiceSuccessResponse<bool>();
        }

        public async Task<ServiceResponse<bool>> DeleteProductAttributeMappingByIdAsync(int id)
        {
            try
            {
                await _productAttributeMappingRepository.DeleteAsync(id);
                return new ServiceSuccessResponse<bool>();
            }
            catch (Exception ex)
            {
                return new ServiceErrorResponse<bool>(ex.Message);
            }
        }

        public async Task<ServiceResponse<List<ProductAttributeValue>>> GetProductAttributeValuesAsync(int productAttributeMappingId)
        {
            var productAttributeValue = await _productAttributeValueRepository.Table
                                        .Where(pav => pav.ProductAttributeMappingId == productAttributeMappingId)
                                        .ToListAsync();

            if (productAttributeValue.Count == 0 || productAttributeValue is null)
                return new ServiceErrorResponse<List<ProductAttributeValue>>("Product Attribute Value Not Found");

            return new ServiceSuccessResponse<List<ProductAttributeValue>>(productAttributeValue);
        }

        public async Task<ServiceResponse<ProductAttributeValue>> GetProductAttributeValuesByIdAsync(int id)
        {
            {
                var pav = await _productAttributeValueRepository.GetByIdAsync(id);
                return new ServiceSuccessResponse<ProductAttributeValue>(pav);
            }
        }

        public async Task<ServiceResponse<bool>> CreateProductAttributeValueAsync(ProductAttributeValue productAttributeValue)
        {
            await _productAttributeValueRepository.CreateAsync(productAttributeValue);
            return new ServiceSuccessResponse<bool>();
        }

        public async Task<ServiceResponse<bool>> UpdateProductAttributeValueAsync(ProductAttributeValue productAttributeValue)
        {
            await _productAttributeValueRepository.UpdateAsync(productAttributeValue);
            return new ServiceSuccessResponse<bool>();
        }

        public async Task<ServiceResponse<bool>> DeleteProductAttributeValueAsync(int id)
        {
            try
            {
                await _productAttributeValueRepository.DeleteAsync(id);
                return new ServiceSuccessResponse<bool>();
            }
            catch (Exception ex)
            {
                return new ServiceErrorResponse<bool>(ex.Message);
            }
        }
    }
}
