//using AutoMapper;
//using Microsoft.AspNetCore.Mvc;
//using T.Library.Model;
//using T.Library.Model.Common;
//using T.Library.Model.Enum;
//using T.Library.Model.Slider;
//using T.Web.Areas.Admin.Models;
//using T.Web.Attribute;
//using T.Web.Controllers;
//
//using T.Web.Services.SliderItemService;

//namespace T.Web.Areas.Admin.Controllers
//{
//    [Area("Admin")]
//    [Route("/admin/slider-item/[action]")]
//    [CustomAuthorizationFilter(RoleName.Admin)]
//    public class SliderController : BaseController
//    {
//        private readonly ISliderItemService _sliderItemService;
//        private readonly IMapper _mapper;
//        private readonly IPrepareModelService _prepareModelService;

//        public SliderController(ISliderItemService sliderItemService, IMapper mapper, IPrepareModelService prepareModelService)
//        {
//            _sliderItemService = sliderItemService;
//            _mapper = mapper;
//            _prepareModelService = prepareModelService;
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetAllAsync()
//        {
//            var sliderItemList = await _sliderItemService.GetAllSliderItemAsync();

//            return Json(new { data = sliderItemList });
//        }

//        [HttpGet]
//        public IActionResult Create()
//        {
//            var model = new SaveSliderItemRequest()
//            {
//                SliderItemQuantity = 1
//            };
//            return View(model);
//        }

//        [HttpPost]
//        public async Task<IActionResult> Create(SaveSliderItemRequest model)
//        {
//            if (!ModelState.IsValid)
//            {
//                return View(model);
//            }

//            var result = await _sliderItemService.CreateAsync(model);

//            if (!result.Success)
//            {
//                ModelState.AddModelError(string.Empty, result.Message);
//                return View(model);
//            }

//            SetStatusMessage(result.Message);
//            return RedirectToAction(nameof(Create));
//        }
//    }
//}
