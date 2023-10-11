//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using T.Library.Model;
//using T.Library.Model.Response;
//using T.Library.Model.Roles.RoleName;
//using T.WebApi.Attribute;
//using T.WebApi.Services.CategoryServices;
//using T.WebApi.Services.ProductServices;

//namespace T.WebApi.Controllers
//{
//    [Route("api/product-category")]
//    [ApiController]
//    public class ProductCategoryController : ControllerBase
//    {
//        private readonly IProductCategoryService _productCategoryService;
//        public ProductCategoryController(IProductCategoryService productCategoryService)
//        {
//            _productCategoryService = productCategoryService;
//        }

//        [HttpGet(APIRoutes.GetAll)]
//        public async Task<ActionResult> GetAll() 
//        {
//            return Ok(await _productCategoryService.GetAllProductCategoryAsync());
//        }

//        [HttpGet("{id}")]
//        public async Task<ActionResult<ServiceResponse<ProductCategory>>> Get(int id)
//        {
//            return await _productCategoryService.GetProductCategoryById(id);
//        }

//        [HttpGet("{productId}/by-productId")]
//        public async Task<ActionResult<ServiceResponse<List<ProductCategory>>>> GetByProductId(int productId)
//        {
//            return await _productCategoryService.GetProductCategoriesByProductId(productId);
//        }

//        [HttpGet("{categoryId}/by-categoryId")]
//        public async Task<ActionResult<ServiceResponse<List<ProductCategory>>>> GetByCategoryId(int categoryId)
//        {
//            return await _productCategoryService.GetProductCategoriesByCategoryId(categoryId);
//        }

//        [HttpPost(APIRoutes.AddOrEdit)]
//        [ServiceFilter(typeof(ValidationFilterAttribute))]
//        public async Task<ActionResult> CreateOrEdit(ProductCategory productCategory)
//        {
//            var result = await _productCategoryService.UpdateProductCategoryAsync(productCategory);
//            if(!result.Success)
//                return BadRequest(result);

//            return Ok(result);
//        }

//        [HttpDelete("delete/{id}")]
//        public async Task<ActionResult> Delete(int id)
//        {
//            var result = await _productCategoryService.DeleteProductCategoryAsync(id);
//            if (!result.Success)
//                return BadRequest(result);

//            return Ok(result);
//        }
//    }
//}
