using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using T.Library.Model;
using T.Library.Model.Interface;
using T.Library.Model.Response;
using T.Library.Model.Roles.RoleName;
using T.WebApi.Attribute;
using T.WebApi.Services.CategoryServices;
using T.WebApi.Services.ProductServices;

namespace T.WebApi.Controllers
{
    [Route("api/product-categories")]
    [ApiController]
    public class ProductCategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public ProductCategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("by-category-id/{categoryId}")]
        public async Task<ActionResult<List<ProductCategory>>> GetProductCategoriesByCategoryIdAsync(int categoryId)
        {
            return await _categoryService.GetProductCategoriesByCategoryIdAsync(categoryId);
        }

        [HttpGet("by-product-id/{productId}")]
        public async Task<ActionResult<List<ProductCategory>>> GetProductCategoriesByProductIdAsync(int productId)
        {
            return await _categoryService.GetProductCategoriesByProductIdAsync(productId);
        }

        [HttpPost("bulk")]
        public async Task<ActionResult> BulkCreateProductCategoriesAsync(List<ProductCategory> productCategories)
        {
            var result = await _categoryService.BulkCreateProductCategoriesAsync(productCategories);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> UpdateProductCategoryAsync(ProductCategory productCategory)
        {
            var result = await _categoryService.UpdateProductCategoryAsync(productCategory);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> DeleteCategoryMappingById(int id)
        {
            var result = await _categoryService.DeleteProductCategoryMappingById(id);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult<ProductCategory>> GetProductCategoryByIdAsync(int id)
        {
            return Ok(await _categoryService.GetProductCategoryByIdAsync(id));
        }
    }
}
