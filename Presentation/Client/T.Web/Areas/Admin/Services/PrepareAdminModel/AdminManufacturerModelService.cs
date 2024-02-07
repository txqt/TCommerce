using T.Web.Areas.Admin.Models.SearchModel;
using T.Web.Services.PrepareModelServices.PrepareAdminModel;

namespace T.Web.Areas.Admin.Services.PrepareAdminModel
{
    public interface IAdminManufacturerModelService
    {
        Task<ProductManufacturerSearchModel> PrepareAddProductToManufacturerSearchModel(ProductManufacturerSearchModel model);
    }
    public class AdminManufacturerModelService : IAdminManufacturerModelService
    {
        private readonly IBaseAdminModelService _baseAdminModelService;

        public AdminManufacturerModelService(IBaseAdminModelService baseAdminModelService)
        {
            _baseAdminModelService = baseAdminModelService;
        }

        public async Task<ProductManufacturerSearchModel> PrepareAddProductToManufacturerSearchModel(ProductManufacturerSearchModel model)
        {
            await _baseAdminModelService.PrepareSelectListCategoryAsync(model.AvailableCategories);
            await _baseAdminModelService.PrepareSelectListManufactureAsync(model.AvailableManufacturers);
            return model;
        }
    }
}
