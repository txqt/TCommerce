//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using T.Library.Model.BannerItem;
//using T.Library.Model.Common;
//using T.Library.Model.Enum;
//using T.Library.Model.Slider;
//using T.WebApi.Attribute;
//using T.WebApi.Services.SliderItemServices;

//namespace T.WebApi.Controllers
//{
//    [Route("api/slider-item")]
//    [ApiController]
//    [CustomAuthorizationFilter(RoleName.Admin)]
//    public class SliderItemController : ControllerBase
//    {
//        private readonly ISliderItemService _sliderItemService;

//        public SliderItemController(ISliderItemService sliderItemService)
//        {
//            _sliderItemService = sliderItemService;
//        }

//        [HttpGet(APIRoutes.GetAll)]
//        [AllowAnonymous]
//        public async Task<ActionResult> GetAllSliderItemAsync()
//        {
//            return Ok(await _sliderItemService.GetAllSliderItemAsync());
//        }

//        [HttpPost("create")]
//        [ServiceFilter(typeof(ValidationFilterAttribute))]
//        public async Task<ActionResult> CreateSliderItemAsync(SaveSliderItemRequest saveSliderItemRequest)
//        {
//            var result = await _sliderItemService.CreateAsync(saveSliderItemRequest);
//            if (!result.Success)
//                return BadRequest(result);

//            return Ok(result);
//        }

//        [HttpDelete("delete-all")]
//        public async Task<ActionResult> DeleteSliderItemAllAsync()
//        {
//            var result = await _sliderItemService.DeleteAllSliderItemAsync();
//            if (!result.Success)
//                return BadRequest(result);

//            return Ok(result);
//        }
//    }
//}
