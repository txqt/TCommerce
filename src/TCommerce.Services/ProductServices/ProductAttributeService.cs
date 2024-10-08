﻿using Microsoft.EntityFrameworkCore;
using TCommerce.Core.Extensions;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Core.Models.Response;
using TCommerce.Services.IRepositoryServices;

namespace TCommerce.Services.ProductServices
{
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
                .Sort(productAttributeParameters.OrderBy ?? string.Empty)
                .ToListAsync();

            return PagedList<ProductAttribute>
                        .ToPagedList(list_product_attribute, productAttributeParameters.PageNumber, productAttributeParameters.PageSize);
        }

        public async Task<List<ProductAttribute>> GetAllProductAttributeAsync()
        {
            return (await _productAttributeRepository.GetAllAsync()).ToList();
        }

        public async Task<ProductAttribute?> GetProductAttributeByIdAsync(int id)
        {
            return await _productAttributeRepository.GetByIdAsync(id);
        }

        public async Task<ProductAttribute?> GetProductAttributeByName(string name)
        {
            return await _productAttributeRepository.Table
                    .FirstOrDefaultAsync(x => x.Name == name);
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

        public async Task<ProductAttributeMapping?> GetProductAttributeMappingByIdAsync(int id)
        {
            return await _productAttributeMappingRepository.GetByIdAsync(id);
        }

        public async Task<List<ProductAttributeMapping>> GetProductAttributesMappingByProductIdAsync(int id)
        {
            var list = await _productAttributeMappingRepository.Table.Where(x => x.ProductId == id)
                .Include(x => x.ProductAttribute)
                .ToListAsync();
            return list;
        }

        public async Task<ServiceResponse<bool>> CreateProductAttributeMappingAsync(ProductAttributeMapping productAttributeMapping)
        {
            try
            {
                if(productAttributeMapping.AttributeControlTypeId <= 0)
                {
                    productAttributeMapping.AttributeControlType = AttributeControlType.DropdownList;
                }

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
                if (productAttributeMapping.AttributeControlTypeId <= 0)
                {
                    productAttributeMapping.AttributeControlType = AttributeControlType.DropdownList;
                }
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

        public async Task<List<ProductAttributeValue>> GetProductAttributeValuesByMappingIdAsync(int productAttributeMappingId)
        {
            return await _productAttributeValueRepository.Table
                                        .Where(pav => pav.ProductAttributeMappingId == productAttributeMappingId)
                                        .ToListAsync();
        }

        public async Task<ProductAttributeValue?> GetProductAttributeValuesByIdAsync(int id)
        {
            return await _productAttributeValueRepository.GetByIdAsync(id);
        }

        public async Task<ServiceResponse<bool>> CreateProductAttributeValueAsync(ProductAttributeValue productAttributeValue)
        {
            if (productAttributeValue.ColorSquaresRgb is not null && !productAttributeValue.ColorSquaresRgb.Contains("#"))
            {
                productAttributeValue.ColorSquaresRgb.Insert(0, "#");
            }
            await _productAttributeValueRepository.CreateAsync(productAttributeValue);
            return new ServiceSuccessResponse<bool>();
        }

        public async Task<ServiceResponse<bool>> UpdateProductAttributeValueAsync(ProductAttributeValue productAttributeValue)
        {
            if (productAttributeValue.ColorSquaresRgb is not null && !productAttributeValue.ColorSquaresRgb.Contains("#"))
            {
                productAttributeValue.ColorSquaresRgb.Insert(0, "#");
            }
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
