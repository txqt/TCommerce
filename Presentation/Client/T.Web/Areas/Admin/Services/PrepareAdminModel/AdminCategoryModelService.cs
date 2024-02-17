using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using T.Library.Model;
using T.Library.Model.Catalogs;
using T.Library.Model.Interface;
using T.Web.Areas.Admin.Models;
using T.Web.Areas.Admin.Models.SearchModel;
using T.Web.Services.PrepareModelServices.PrepareAdminModel;
using T.Web.Services.UrlRecordService;

namespace T.Web.Areas.Admin.Services.PrepareAdminModel
{
    public interface IAdminCategoryModelService
    {
        Task<CategoryModelAdmin> PrepareCategoryModelAsync(CategoryModelAdmin model, Category category);
        Task<ProductCategorySearchModel> PrepareAddProductToCategorySearchModel(ProductCategorySearchModel model);
    }
    public class AdminCategoryModelService : IAdminCategoryModelService
    {
        private readonly IMapper _mapper;
        private readonly ICategoryServiceCommon _categoryService;
        private readonly IBaseAdminModelService _baseAdminModelService;
        private readonly IUrlRecordService _urlRecordService;

        public AdminCategoryModelService(IMapper mapper, ICategoryServiceCommon categoryService, IBaseAdminModelService baseAdminModelService, IUrlRecordService urlRecordService)
        {
            _mapper = mapper;
            _categoryService = categoryService;
            _baseAdminModelService = baseAdminModelService;
            _urlRecordService = urlRecordService;
        }

        public async Task<CategoryModelAdmin> PrepareCategoryModelAsync(CategoryModelAdmin model, Category category)
        {
            if (category is not null)
            {
                model ??= new CategoryModelAdmin()
                {
                    Id = category.Id
                };
                _mapper.Map(category, model);
                model.SeName = await _urlRecordService.GetSeNameAsync(category);
            }

            await _baseAdminModelService.PrepareSelectListCategoryAsync(model.AvailableCategories);

            return model;
        }

        public async Task<ProductCategorySearchModel> PrepareAddProductToCategorySearchModel(ProductCategorySearchModel model)
        {
            await _baseAdminModelService.PrepareSelectListCategoryAsync(model.AvailableCategories);
            return model;
        }
    }
}
