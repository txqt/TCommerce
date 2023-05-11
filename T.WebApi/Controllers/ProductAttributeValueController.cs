using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using T.WebApi.Services.ProductServices;

namespace T.WebApi.Controllers
{
    [Route("api/product-attribute-value")]
    [ApiController]
    public class ProductAttributeValueController : ControllerBase
    {
        private readonly IProductAttributeValueService _productAttributeValueService;
        public ProductAttributeValueController(IProductAttributeValueService productAttributeValueService)
        {
            _productAttributeValueService = productAttributeValueService;
        }

        [HttpGet("get-by-mapping-id/{productAttributeMappingId}")]
        public async Task<IActionResult> GetProductAttributeValuesAsync(int productAttributeMappingId)
        {
            var result = await _productAttributeValueService.GetProductAttributeValuesAsync(productAttributeMappingId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

    }
}
