using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using T.Library.Model;
using T.WebApi.Attribute;
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductAttributeValuesByIdAsync(int id)
        {
            var result = await _productAttributeValueService.GetProductAttributeValuesByIdAsync(id);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost(APIRoutes.AddOrEdit)]
        public async Task<ActionResult> AddProductAttributeMapping(ProductAttributeValue productAttributeValue)
        {
            var result = await _productAttributeValueService.AddOrUpdateProductAttributeValue(productAttributeValue);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("delete/{productAttributeValueId}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> DeleteProductAttributeMapping(int productAttributeValueId)
        {
            var result = await _productAttributeValueService.DeleteProductAttributeValue(productAttributeValueId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
