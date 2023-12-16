using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using T.Library.Model;
using T.Library.Model.Response;
using T.Library.Model.Roles.RoleName;
using T.Library.Model.Security;
using T.WebApi.Attribute;
using T.WebApi.Services.ProductServices;

namespace T.WebApi.Controllers
{
    [Route("api/product-attribute-mappings")]
    [ApiController]
    [CheckPermission(PermissionSystemName.ManageProducts)]
    public class ProductAttributeMappingController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IProductAttributeService _productAttributeService;

        public ProductAttributeMappingController(IProductService productService, IProductAttributeService productAttributeService)
        {
            _productService = productService;
            _productAttributeService = productAttributeService;
        }

        [AllowAnonymous]
        [HttpGet("by-product-id/{productId}")]
        public async Task<ActionResult> GetProductAttributesMapping(int productId)
        {
            return Ok(await _productAttributeService.GetProductAttributesMappingByProductIdAsync(productId));
        }

        [HttpPost("")]
        public async Task<ActionResult> CreateProductAttributeMapping(ProductAttributeMapping productAttributeMapping)
        {

            var result = await _productAttributeService.CreateProductAttributeMappingAsync(productAttributeMapping);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("")]
        public async Task<ActionResult> UpdateAttributeMapping(ProductAttributeMapping productAttributeMapping)
        {
            //try to get a product with the specified id
            var product = (await _productService.GetByIdAsync(productAttributeMapping.ProductId)) ??
              throw new ArgumentException("No product found with the specified id");

            var productAttribute = (await _productAttributeService.GetProductAttributeByIdAsync(productAttributeMapping.ProductAttributeId)) ??
                throw new ArgumentException("No product attribute found with the specified id");

            var result = await _productAttributeService.UpdateProductAttributeMappingAsync(productAttributeMapping);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<ProductAttributeMapping>>> GetProductAttributeMappingByIdAsync(int id)
        {
            return Ok(await _productAttributeService.GetProductAttributeMappingByIdAsync(id));
        }

        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> DeleteProductAttributeMapping(int id)
        {
            var result = await _productAttributeService.DeleteProductAttributeMappingByIdAsync(id);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        #region ProductAttributeValue
        [AllowAnonymous]
        [HttpGet("{id}/value")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetAttributeValues(int id)
        {
            var result = await _productAttributeService.GetProductAttributeValuesAsync(id);

            if (result is null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("value/{valueId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAttributeValueById(int valueId)
        {
            var result = await _productAttributeService.GetProductAttributeValuesByIdAsync(valueId);

            if (result is null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost("value")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateProductAttributeValue(ProductAttributeValue productAttributeValue)
        {
            var result = await _productAttributeService.CreateProductAttributeValueAsync(productAttributeValue);

            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("value")]
        public async Task<ActionResult> UpdateProductAttributeValue(ProductAttributeValue productAttributeValue)
        {
            var result = await _productAttributeService.UpdateProductAttributeValueAsync(productAttributeValue);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("value/{valueId}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> DeleteProductAttributeValue(int valueId)
        {
            var result = await _productAttributeService.DeleteProductAttributeValueAsync(valueId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion
    }
}
