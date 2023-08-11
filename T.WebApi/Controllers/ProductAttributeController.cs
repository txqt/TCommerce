using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using T.Library.Model;
using T.Library.Model.Response;
using T.Library.Model.Roles.RoleName;
using T.Library.Model.ViewsModel;
using T.WebApi.Attribute;
using T.WebApi.Services.ProductServices;

namespace T.WebApi.Controllers
{
    [Route("api/product-attribute")]
    [ApiController]
    [CustomAuthorizationFilter(RoleName.Admin)]
    public class ProductAttributeController : ControllerBase
    {
        private readonly IProductAttributeService _productAttributeSvc;
        public ProductAttributeController(IProductAttributeService productAttributeSvc)
        {
            _productAttributeSvc = productAttributeSvc;
        }

        [HttpGet(APIRoutes.GetAll)]
        public async Task<IActionResult> GetAllAsync()
        {
            return Ok(await _productAttributeSvc.GetAllProductAttributeAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<ProductAttribute>>> Get(int id)
        {
            return await _productAttributeSvc.GetProductAttributeByIdAsync(id);
        }

        [HttpPost(APIRoutes.Create)]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateAsync(ProductAttribute productAttribute)
        {
            var result = await _productAttributeSvc.CreateProductAttributeAsync(productAttribute);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPost("edit")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> EditAsync(ProductAttribute productAttribute)
        {
            var result = await _productAttributeSvc.UpdateProductAttributeAsync(productAttribute);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("delete/{Id}")]
        public async Task<ActionResult> DeleteProduct(int Id)
        {
            var result = await _productAttributeSvc.DeleteProductAttributeByIdAsync(Id);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
