using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using T.Library.Model;
using T.Library.Model.Response;
using T.Library.Model.Roles.RoleName;
using T.Library.Model.Security;
using T.Library.Model.ViewsModel;
using T.WebApi.Attribute;
using T.WebApi.Services.ProductServices;

namespace T.WebApi.Controllers
{
    [Route("api/product")]
    [ApiController]
    [CustomAuthorizationFilter(RoleName.Admin)]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [AllowAnonymous]
        [HttpGet(APIRoutes.GetAll)]
        [AuthorizePermission(PermissionSystemName.ManageProducts)]
        public async Task<ActionResult<List<Product>>> GetAll([FromQuery] ProductParameters productParameters)
        {
            var products = await _productService.GetAll(productParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(products.MetaData));
            return Ok(products);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ServiceResponse<Product>>> Get(int id)
        {
            return await _productService.GetByIdAsync(id);
        }

        [HttpPost(APIRoutes.Create)]
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

        [HttpPost("edit")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> EditProduct(ProductModel product)
        {
            var result = await _productService.EditProduct(product);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("delete/{productId}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> DeleteProduct(int productId)
        {
            var result = await _productService.DeleteProduct(productId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{id}/attributes")]
        public async Task<ActionResult> GetAllAttribute(int id)
        {
            var result = await _productService.GetAllProductAttribute(id);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("{productId}/all-pictures")]
        [AllowAnonymous]
        public async Task<ActionResult<ServiceResponse<List<ProductPicture>>>> GetProductPicturesByProductIdAsync(int productId)
        {
            return await _productService.GetProductPicturesByProductIdAsync(productId);
        }

        [HttpPost("{productId}/add-new-picture")]
        //[AllowAnonymous]
        public async Task<ActionResult> AddProductImage(List<IFormFile> formFiles, int productId)
        {
            var result = await _productService.AddProductImage(formFiles, productId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("{productId}/delete-picture/{pictureId}")]
        public async Task<ActionResult> DeletePictureProduct(int productId, int pictureId)
        {
            var result = await _productService.DeleteProductImage(productId, pictureId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("{productId}/delete-all-picture")]
        public async Task<ActionResult> DeleteAllPictureProduct(int productId)
        {
            var result = await _productService.DeleteAllProductImage(productId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
