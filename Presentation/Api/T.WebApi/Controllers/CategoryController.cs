using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using T.Library.Model.Catalogs;
using T.Library.Model.Security;
using T.Library.Model.ViewsModel;
using T.WebApi.Attribute;
using T.WebApi.Services.CategoryServices;

namespace T.WebApi.Controllers
{
    [Route("api/categories")]
    [ApiController]
    [CheckPermission(PermissionSystemName.ManageCategories)]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet()]
        public async Task<ActionResult> GetAll() 
        {
            return Ok(await _categoryService.GetAllCategoryAsync());
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Category>> Get(int id)
        {
            return await _categoryService.GetCategoryByIdAsync(id);
        }

        [HttpPost()]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> CreateCategoryAsync(CategoryModel category)
        {
            var result = await _categoryService.CreateCategoryAsync(category);
            if(!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut()]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> UpdateCategoryAsync(CategoryModel category)
        {
            var result = await _categoryService.UpdateCategoryAsync(category);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _categoryService.DeleteCategoryByIdAsync(id);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        //[HttpGet("{categoryId}/product-categories")]
        //public async Task<ActionResult<List<ProductCategory>>> GetProductCategoriesByCategoryIdAsync(int categoryId)
        //{
        //    return await _categoryService.GetProductCategoriesByCategoryIdAsync(categoryId);
        //}

        //[HttpPost("bulk-product-categories")]
        //public async Task<ActionResult> BulkCreateProductCategoriesAsync(List<ProductCategory> productCategories)
        //{
        //    var result = await _categoryService.BulkCreateProductCategoriesAsync(productCategories);
        //    if (!result.Success)
        //        return BadRequest(result);

        //    return Ok(result);
        //}

        //[HttpPut("product-category")]
        //[ServiceFilter(typeof(ValidationFilterAttribute))]
        //public async Task<ActionResult> UpdateProductCategoryAsync(ProductCategory productCategory)
        //{
        //    var result = await _categoryService.UpdateProductCategoryAsync(productCategory);
        //    if (!result.Success)
        //        return BadRequest(result);

        //    return Ok(result);
        //}

        //[HttpDelete("product-category/{productCategoryId}")]
        //[ServiceFilter(typeof(ValidationFilterAttribute))]
        //public async Task<ActionResult> DeleteCategoryMappingById(int productCategoryId)
        //{
        //    var result = await _categoryService.DeleteCategoryMappingById(productCategoryId);
        //    if (!result.Success)
        //        return BadRequest(result);

        //    return Ok(result);
        //}

        //[HttpGet("product-category/{productCategoryId}")]
        //[ServiceFilter(typeof(ValidationFilterAttribute))]
        //public async Task<ActionResult<ProductCategory>> GetProductCategoryByIdAsync(int productCategoryId)
        //{
        //    return Ok(await _categoryService.GetProductCategoryByIdAsync(productCategoryId));
        //}
    }
}
