using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using T.Library.Model;
using T.Library.Model.Interface;
using T.Library.Model.Response;
using T.Library.Model.Roles.RoleName;
using T.Library.Model.Security;
using T.Library.Model.ViewsModel;
using T.WebApi.Attribute;
using T.WebApi.Services.ProductServices;

namespace T.WebApi.Controllers
{
    [Route("api/product-attributes")]
    [ApiController]
    [CheckPermission(PermissionSystemName.ManageAttributes)]
    public class ProductAttributeController : ControllerBase
    {
        private readonly IProductAttributeService _productAttributeSvc;
        public ProductAttributeController(IProductAttributeService productAttributeSvc)
        {
            _productAttributeSvc = productAttributeSvc;
        }

        [HttpGet()]
        public async Task<IActionResult> GetAllAsync()
        {
            return Ok(await _productAttributeSvc.GetAllProductAttributeAsync());
        }

        [AllowAnonymous]
        [HttpGet(APIRoutes.GETALL + "/paged")]
        public async Task<IActionResult> GetAllPagedAsync([FromQuery]ProductAttributeParameters productAttributeParameters)
        {
            var products = await _productAttributeSvc.GetAllPagedAsync(productAttributeParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(products.MetaData));
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductAttribute>> Get(int id)
        {
            return await _productAttributeSvc.GetProductAttributeByIdAsync(id);
        }

        [HttpPost()]
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
        [HttpPut("")]
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

        [HttpDelete("id")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var result = await _productAttributeSvc.DeleteProductAttributeByIdAsync(id);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
