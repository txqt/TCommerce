using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using T.Library.Model.Common;
using T.Library.Model.Interface;
using T.Library.Model.Response;
using T.Library.Model.Roles.RoleName;
using T.Library.Model.Security;
using T.WebApi.Attribute;
using T.WebApi.Services.CategoryServices;

namespace T.WebApi.Controllers
{
    [Route("api/category")]
    [ApiController]
    [CheckPermission(PermissionSystemName.ManageCategories)]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet(APIRoutes.GetAll)]
        public async Task<ActionResult> GetAll() 
        {
            return Ok(await _categoryService.GetAllCategoryAsync());
        }

        [HttpGet("{categoryId}")]
        public async Task<ActionResult<ServiceResponse<Category>>> Get(int categoryId)
        {
            return await _categoryService.GetCategoryByIdAsync(categoryId);
        }

        [HttpPost()]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> CreateCategoryAsync(Category category)
        {
            var result = await _categoryService.CreateCategoryAsync(category);
            if(!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut()]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> UpdateCategoryAsync(Category category)
        {
            var result = await _categoryService.UpdateCategoryAsync(category);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{categoryId}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> Delete(int categoryId)
        {
            var result = await _categoryService.DeleteCategoryByIdAsync(categoryId);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
