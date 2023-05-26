﻿using T.Library.Model.Response;
using T.Library.Model;
using System.Net.Http.Json;

namespace T.Web.Services.ProductService
{
    public interface IProductCategoryService
    {
        Task<List<ProductCategory>> GetAllAsync();
        Task<ServiceResponse<ProductCategory>> Get(int id);
        Task<ServiceResponse<ProductCategory>> GetByProductId(int productId);
        Task<ServiceResponse<ProductCategory>> GetByCategoryId(int categoryId);
        Task<ServiceResponse<bool>> AddOrEdit(ProductCategory productCategory);
        Task<ServiceResponse<bool>> Delete(int id);
    }
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly HttpClient _httpClient;

        public ProductCategoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ServiceResponse<bool>> AddOrEdit(ProductCategory productCategory)
        {
            var result = await _httpClient.PostAsJsonAsync($"api/product-category/{APIRoutes.AddOrEdit}", productCategory);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<bool>> Delete(int id)
        {
            var result = await _httpClient.DeleteAsync($"api/product-category/delete/{id}");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<ProductCategory>> Get(int id)
        {
            var result = await _httpClient.GetAsync($"api/product-category/{id}");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<ProductCategory>>();
        }

        public async Task<List<ProductCategory>> GetAllAsync()
        {
            var result = await _httpClient.GetAsync($"api/product-category/{APIRoutes.GetAll}");
            return await result.Content.ReadFromJsonAsync<List<ProductCategory>>();
        }

        public async Task<ServiceResponse<ProductCategory>> GetByCategoryId(int categoryId)
        {
            var result = await _httpClient.GetAsync($"api/product-category/{categoryId}/by-productId");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<ProductCategory>>();
        }

        public async Task<ServiceResponse<ProductCategory>> GetByProductId(int productId)
        {
            var result = await _httpClient.GetAsync($"api/product-category/{productId}/by-categoryId");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<ProductCategory>>();
        }
    }
}
