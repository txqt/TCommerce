using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using T.Library.Model;
using T.Library.Model.Response;
using T.WebApi.Services.ProductServices;

namespace T.WebApi.Controllers
{
    [Route("api/product-attribute-mapping")]
    [ApiController]
    public class ProductAttributeMappingController : ControllerBase
    {
        private readonly IProductAttributeMappingService _productAttributeMappingService;

        public ProductAttributeMappingController(IProductAttributeMappingService productAttributeMappingService)
        {
            _productAttributeMappingService = productAttributeMappingService;
        }

        [HttpPost("add-or-edit-product-attribute-mapping")]
        public async Task<ActionResult> AddProductAttributeMapping(ProductAttributeMapping productAttributeMapping)
        {
            var result = await _productAttributeMappingService.AddOrUpdateProductAttributeMapping(productAttributeMapping);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [AllowAnonymous]
        [HttpGet("{productAttributeMappingId}/value")]
        public async Task<ActionResult> GetAllValueAttribute(int productAttributeMappingId)
        {
            var result = await _productAttributeMappingService.GetAllValueProductAttribute(productAttributeMappingId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ServiceResponse<ProductAttributeMapping>>> Get(int id)
        {
            return await _productAttributeMappingService.GetProductAttributeMappingByIdAsync(id);
        }
        [HttpGet("get-by-product-id/{productId}")]
        [AllowAnonymous]
        public async Task<ActionResult<ServiceResponse<List<ProductAttributeMapping>>>> GetByProductId(int productId)
        {
            return await _productAttributeMappingService.GetProductAttributeMappingByProductIdAsync(productId);
        }
    }
}
