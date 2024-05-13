using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using T.Library.Model.Banners;
using T.Library.Model.Interface;
using T.Library.Model.ViewsModel;
using T.Web.Areas.Admin.Models;
using T.Web.Areas.Admin.Services.PrepareAdminModel;
using T.Web.Extensions;

namespace T.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("/admin/banner/[action]")]
    public class BannerController : BaseAdminController
    {
        private readonly IBannerService _bannerService;
        private readonly IAdminBannerModelService _prepareModelService;
        private readonly IMapper _mapper;
        public BannerController(IBannerService bannerService, IAdminBannerModelService prepareModelService, IMapper mapper)
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
                EditUrl = Url.Action("Edit", "Banner"),
                DeleteUrl = Url.Action("Delete", "Banner"),
                GetDataUrl = Url.Action("GetAll", "Banner"),
                Columns = new List<ColumnDefinition>
                {
                    new ColumnDefinition { Data = nameof(Banner.Title), Title = DisplayNameExtensions.GetPropertyDisplayName<Banner>(m=>m.Title) },
                    new ColumnDefinition { Data = nameof(Banner.Subtitle), Title = DisplayNameExtensions.GetPropertyDisplayName<Banner>(m=>m.Subtitle) },
                    new ColumnDefinition { Data = nameof(Banner.Price), Title = DisplayNameExtensions.GetPropertyDisplayName<Banner>(m=>m.Price) },
                    new ColumnDefinition { Data = $"{nameof(Banner.Picture)}.{nameof(Banner.Picture.UrlPath)}", Title = DisplayNameExtensions.GetPropertyDisplayName<Banner>(m=>m.Picture), RenderType = RenderType.RenderPicture },
                    new ColumnDefinition(nameof(Banner.Id)) { RenderType = RenderType.RenderButtonEdit },
                    new ColumnDefinition(nameof(Banner.Id)) { RenderType = RenderType.RenderButtonRemove },
                }
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _bannerService.GetAllBannerAsync();
            var json = new { data = result };
            return this.JsonWithPascalCase(json);
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
            var banner = await _bannerService.GetBannerByIdAsync(id) ??
                throw new ArgumentException("Not found with the specified id");

            var model = _prepareModelService.PrepareBannerModel(new BannerViewModel(), banner);

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

            var banner = await _bannerService.GetBannerByIdAsync(model.Id) ??
                throw new ArgumentException("Not found with the specified id");

            //banner = _mapper.Map(model, banner);

            var result = await _bannerService.UpdateBannerAsync(model);
            if (!result.Success)
            {
                SetStatusMessage($"{result.Message}");
                model = _prepareModelService.PrepareBannerModel(model, banner);
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
