using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using T.Library.Model.Common;
using T.Library.Model.Seo;
using T.Library.Model.ViewsModel;
using T.WebApi.Services.UrlRecordServices;

namespace T.WebApi.Controllers
{
    [Route("api/url-records")]
    [ApiController]
    public class UrlRecordController : ControllerBase
    {
        private readonly IUrlRecordService _urlRecordService;

        public UrlRecordController(IUrlRecordService urlRecordService)
        {
            _urlRecordService = urlRecordService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllAsync()
        {
            return Ok(await _urlRecordService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetByIdAsync(int id)
        {
            return Ok(await _urlRecordService.GetByIdAsync(id));
        }

        [HttpPost()]
        public async Task<ActionResult> CreateUrlRecordAsync([FromForm] UrlRecord model)
        {
            var result = await _urlRecordService.CreateUrlRecordAsync(model);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPut()]
        public async Task<ActionResult> UpdateUrlRecordAsync([FromForm] UrlRecord model)
        {
            var result = await _urlRecordService.UpdateUrlRecordAsync(model);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBannerDeleteUrlRecordByIdAsyncByIdAsync(int id)
        {
            var result = await _urlRecordService.DeleteUrlRecordByIdAsync(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("slug/{slug}")]
        public async Task<ActionResult> GetBySlugAsync(string slug)
        {
            return Ok(await _urlRecordService.GetBySlugAsync(slug));
        }
        [HttpGet("active-slug/{entityId}/{entityName}")]
        public async Task<ActionResult> GetActiveSlugForEntityAsync(int entityId, string entityName)
        {
            return Ok(await _urlRecordService.GetActiveSlugAsync(entityId, entityName));
        }
    }
}
