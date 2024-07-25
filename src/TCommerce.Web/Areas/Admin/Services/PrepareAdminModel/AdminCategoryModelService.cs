using AutoMapper;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Services.PrepareModelServices.PrepareAdminModel;
using TCommerce.Core.Interface;
using TCommerce.Web.Areas.Admin.Models.Catalog;

namespace TCommerce.Web.Areas.Admin.Services.PrepareAdminModel
{
    public interface IAdminCategoryModelService
    {
        Task<CategoryModel> PrepareCategoryModelAsync(CategoryModel model, Category category);
        Task<ProductCategorySearchModel> PrepareAddProductToCategorySearchModel(ProductCategorySearchModel model);
    }
    public class AdminCategoryModelService : IAdminCategoryModelService
    {
        private readonly IMapper _mapper;
        private readonly ICategoryService _categoryService;
        private readonly IBaseAdminModelService _baseAdminModelService;
        private readonly IUrlRecordService _urlRecordService;

        public AdminCategoryModelService(IMapper mapper, ICategoryService categoryService, IBaseAdminModelService baseAdminModelService, IUrlRecordService urlRecordService)
        {
            _mapper = mapper;
            _categoryService = categoryService;
            _baseAdminModelService = baseAdminModelService;
            _urlRecordService = urlRecordService;
        }

        public async Task<CategoryModel> PrepareCategoryModelAsync(CategoryModel model, Category category)
        {
            if (category is not null)
            {
                model ??= new CategoryModel()
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
            await _baseAdminModelService.PrepareSelectListManufactureAsync(model.AvailableManufacturers);
            return model;
        }
    }
}
