using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using T.WebApi.Attribute;
using T.WebApi.Services.DiscountServices;

namespace T.WebApi.Controllers
{
    [Route("api/discount")]
    [ApiController]
    [CheckPermission]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountService _discountService;

        public DiscountController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        [HttpPost("is-valid")]
        //[ServiceFilter(typeof(ValidationFilterAttribute))]
        [AllowAnonymous]
        public async Task<IActionResult> CheckValidDiscountCode([FromBody] string discountCode)
        {
            var result = await _discountService.CheckValidDiscountCode(discountCode);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("{discountCode}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [AllowAnonymous]
        public async Task<IActionResult> GetDiscountByCode(string discountCode)
        {
            var result = await _discountService.GetDiscountByCode(discountCode);
            return Ok(result);
        }
    }
}
