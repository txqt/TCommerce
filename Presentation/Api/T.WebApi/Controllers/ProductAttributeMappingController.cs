//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using T.Library.Model;
//using T.Library.Model.Response;
//using T.Library.Model.Roles.RoleName;
//using T.WebApi.Attribute;
//using T.WebApi.Services.ProductServices;

//namespace T.WebApi.Controllers
//{
//    [Route("api/product-attribute-mapping")]
//    [ApiController]
//    [CustomAuthorizationFilter(RoleName.Admin)]
//    public class ProductAttributeMappingController : ControllerBase
//    {
//        private readonly IProductAttributeMappingService _productAttributeMappingService;
//        private readonly IProductService _productService;
//        private readonly IProductAttributeService _productAttributeService;

//        public ProductAttributeMappingController(IProductAttributeMappingService productAttributeMappingService, IProductService productService, IProductAttributeService productAttributeService)
//        {
//            _productAttributeMappingService = productAttributeMappingService;
//            _productService = productService;
//            _productAttributeService = productAttributeService;
//        }

//        [HttpPost(APIRoutes.AddOrEdit)]
//        public async Task<ActionResult> AddProductAttributeMapping(ProductAttributeMapping productAttributeMapping)
//        {
//            //try to get a product with the specified id
//            var product = (await _productService.GetByIdAsync(productAttributeMapping.ProductId)).Data ??
//              throw new ArgumentException("No product found with the specified id");

//            var productAttribute = (await _productAttributeService.GetProductAttributeByIdAsync(productAttributeMapping.ProductAttributeId)).Data ??
//                throw new ArgumentException("No product attribute found with the specified id");

//            var result = await _productAttributeMappingService.CreateOrEditProductAttributeMappingAsync(productAttributeMapping);
//            if (!result.Success)
//            {
//                return BadRequest(result);
//            }

//            return Ok(result);
//        }

//        [AllowAnonymous]
//        [HttpGet("{productAttributeMappingId}/value")]
//        public async Task<ActionResult> GetAllValueAttribute(int productAttributeMappingId)
//        {
//            var result = await _productAttributeMappingService.GetAllValueProductAttributeByIdAsync(productAttributeMappingId);
//            if (!result.Success)
//            {
//                return BadRequest(result);
//            }
//            return Ok(result);
//        }

//        [HttpGet("{id}")]
//        [AllowAnonymous]
//        public async Task<ActionResult<ServiceResponse<ProductAttributeMapping>>> Get(int id)
//        {
//            return await _productAttributeMappingService.GetProductAttributeMappingByIdAsync(id);
//        }

//        [HttpGet("get-by-product-id/{productId}")]
//        [AllowAnonymous]
//        public async Task<ActionResult<ServiceResponse<List<ProductAttributeMapping>>>> GetByProductId(int productId)
//        {
//            return await _productAttributeMappingService.GetProductAttributeMappingByProductIdAsync(productId);
//        }

//        [HttpDelete("delete/{productAttributeId}")]
//        [ServiceFilter(typeof(ValidationFilterAttribute))]
//        public async Task<ActionResult> DeleteProductAttributeMapping(int productAttributeId)
//        {
//            var result = await _productAttributeMappingService.DeleteProductAttributeMappingByIdAsync(productAttributeId);
//            if (!result.Success)
//            {
//                return BadRequest(result);
//            }
//            return Ok(result);
//        }
//    }
//}
