using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using T.Library.Model.Banners;
using T.Library.Model.Interface;
using T.Library.Model.ViewsModel;

namespace T.WebApi.Controllers
{
    [Route("api/banner")]
    [ApiController]
    public class BannerController : ControllerBase
    {
        private readonly IBannerService _bannerService;
        public BannerController(IBannerService bannerService)
        {
            _bannerService = bannerService;
        }

        [HttpGet()]
        public async Task<ActionResult> GetAllBannerAsync()
        {
            return Ok(await _bannerService.GetAllBannerAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Banner?>> GetBannerByIdAsync(int id)
        {
            return await _bannerService.GetBannerByIdAsync(id);
        }

        [HttpPost()]
        public async Task<ActionResult> CreateBannerAsync([FromForm] BannerViewModel banner)
        {
            var result = await _bannerService.CreateBannerAsync(banner);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPut()]
        public async Task<ActionResult> UpdateBannerAsync([FromForm] BannerViewModel banner)
        {
            var result = await _bannerService.UpdateBannerAsync(banner);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBannerByIdAsync(int id)
        {
            var result = await _bannerService.DeleteBannerByIdAsync(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
