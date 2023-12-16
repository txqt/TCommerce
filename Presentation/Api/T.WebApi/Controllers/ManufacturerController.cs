using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using T.Library.Model.Catalogs;
using T.Library.Model.Interface;
using T.Library.Model.Response;
using T.WebApi.Attribute;

namespace T.WebApi.Controllers
{
    [Route("api/manufacturer")]
    [ApiController]
    public class ManufacturerController : ControllerBase
    {
        private readonly IManufacturerServicesCommon _manufacturerService;
        public ManufacturerController(IManufacturerServicesCommon manufacturerService)
        {
            _manufacturerService = manufacturerService;
        }

        [HttpGet(APIRoutes.GETALL)]
        public async Task<ActionResult> GetAll()
        {
            return Ok(await _manufacturerService.GetAllManufacturerAsync());
        }

        [HttpGet("{manufacturerId}")]
        public async Task<ActionResult<Manufacturer>> Get(int manufacturerId)
        {
            return await _manufacturerService.GetManufacturerByIdAsync(manufacturerId);
        }

        [HttpPost()]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> CreateManufacturerAsync(Manufacturer manufacturer)
        {
            var result = await _manufacturerService.CreateManufacturerAsync(manufacturer);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut()]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> UpdateManufacturerAsync(Manufacturer manufacturer)
        {
            var result = await _manufacturerService.UpdateManufacturerAsync(manufacturer);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{manufacturerId}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> Delete(int manufacturerId)
        {
            var result = await _manufacturerService.DeleteManufacturerByIdAsync(manufacturerId);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("{manufacturerId}/product-manufacturer")]
        public async Task<ActionResult<List<ProductManufacturer>>> GetProductManufacturersByManufacturerIdAsync(int manufacturerId)
        {
            return await _manufacturerService.GetProductManufacturersByManufacturerIdAsync(manufacturerId);
        }

        [HttpPost("bulk-product-manufacturers")]
        public async Task<ActionResult> BulkCreateProductManufacturersAsync(List<ProductManufacturer> productManufacturers)
        {
            var result = await _manufacturerService.BulkCreateProductManufacturersAsync(productManufacturers);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("product-manufacturer")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> UpdateProductManufacturerAsync(ProductManufacturer productManufacturers)
        {
            var result = await _manufacturerService.UpdateProductManufacturerAsync(productManufacturers);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("product-manufacturer/{manufacturerId}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> DeleteManufacturerByIdAsync(int manufacturerId)
        {
            var result = await _manufacturerService.DeleteManufacturerByIdAsync(manufacturerId);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("product-manufacturer/{manufacturerId}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult<ProductManufacturer>> GetProductManufacturerByIdAsync(int productManufacturerId)
        {
            var result = await _manufacturerService.GetProductManufacturerByIdAsync(productManufacturerId);
            return Ok(result);
        }
    }
}
