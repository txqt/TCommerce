using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using T.Library.Model;
using T.Library.Model.Response;
using T.WebApi.Attribute;
using T.WebApi.Services.ProductServices;

namespace T.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("get-all")]
        public async Task<ActionResult<List<Product>>> GetAll([FromQuery] ProductParameters productParameters)
        {
            var products = await _productService.GetAll(productParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(products.MetaData));
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<Product>>> Get(int id)
        {
            return await _productService.Get(id);
        }

        [HttpPost("create-product")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> CreateProduct(Product product)
        {
            var result = await _productService.CreateProduct(product);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
