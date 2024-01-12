using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using T.WebApi.Services.PictureServices;

namespace T.WebApi.Controllers
{
    [Route("api/pictures")]
    [ApiController]
    public class PictureController : ControllerBase
    {
        private readonly IPictureService _pictureService;

        public PictureController(IPictureService pictureService)
        {
            _pictureService = pictureService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await _pictureService.GetPictureByIdAsync(id));
        }
    }
}
