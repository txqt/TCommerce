using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using T.Library.Model;
using T.WebApi.Attribute;
using T.WebApi.Services.CategoryServices;

namespace T.WebApi.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost("add-or-edit-category")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> Create(Category category)
        {
            var result = await _categoryService.CreateOrEditAsync(category);
            if(!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
