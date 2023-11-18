using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using T.Library.Model.Banners;
using T.Library.Model.Interface;
using T.Library.Model.ViewsModel;
using T.Web.Areas.Admin.Models;
using T.Web.Services.PrepareModelServices;

namespace T.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("/admin/banner/[action]")]
    public class BannerController : BaseAdminController
    {
        private readonly IBannerService _bannerService;
        private readonly IBannerModelService _prepareModelService;
        private readonly IMapper _mapper;
        public BannerController(IBannerService bannerService, IBannerModelService prepareModelService, IMapper mapper)
        {
            _bannerService = bannerService;
            _prepareModelService = prepareModelService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var model = new DataTableViewModel
            {
                TableTitle = "Danh sách Banner",
                CreateUrl = Url.Action("Create", "Banner"),
                //Headers = new List<string> { "Title", "Subtitle", "Price", "Image", "Edit", "Delete"},
                GetDataUrl = Url.Action("GetAll", "Banner"),
                Columns = new List<ColumnDefinition>
                {
                    new ColumnDefinition { Data = "title" },
                    new ColumnDefinition { Data = "subtitle" },
                    new ColumnDefinition { Data = "price" },
                    new ColumnDefinition { Data = "picture.urlPath", RenderType = RenderType.RenderPicture },
                    new ColumnDefinition("id") { EditUrl = Url.Action("Edit", "Banner"), RenderType = RenderType.RenderButtonEdit },
                    new ColumnDefinition("id") { DeleteUrl = Url.Action("Delete", "Banner"), RenderType = RenderType.RenderButtonRemove },
                }
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _bannerService.GetAllBannerAsync();
            return Json(new { data = result });
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new BannerViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(BannerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                AddErrorsFromModel(ModelState.Values);
                return View(model);
            }
            //var banner = _mapper.Map<Banner>(bannerViewModel);

            var result = await _bannerService.CreateBannerAsync(model);
            if (result.Success)
            {
                SetStatusMessage("Thêm banner mới thành công");
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var banner = (await _bannerService.GetBannerByIdAsync(id)).Data ??
                throw new ArgumentException("Not found with the specified id");

            var model = await _prepareModelService.PrepareBannerModelAsync(new BannerViewModel(), banner);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BannerViewModel model)
        {
            ModelState.Remove("ImageFile");
            if (!ModelState.IsValid)
            {
                AddErrorsFromModel(ModelState.Values);
                return View(model);
            }

            var banner = (await _bannerService.GetBannerByIdAsync(model.Id)).Data ??
                throw new ArgumentException("Not found with the specified id");

            //banner = _mapper.Map(model, banner);

            var result = await _bannerService.UpdateBannerAsync(model);
            if (!result.Success)
            {
                SetStatusMessage($"{result.Message}");
                model = await _prepareModelService.PrepareBannerModelAsync(model, banner);
                return View(model);
            }

            SetStatusMessage("Sửa thành công");
            //model = await _prepareModelService.PrepareBannerModelAsync(model, banner);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {

            var result = await _bannerService.DeleteBannerByIdAsync(id);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
