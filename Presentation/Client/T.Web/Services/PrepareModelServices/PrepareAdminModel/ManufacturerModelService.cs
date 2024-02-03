using T.Web.Areas.Admin.Models.SearchModel;

namespace T.Web.Services.PrepareModelServices.PrepareAdminModel
{
    public interface IManufacturerModelService
    {
        Task<AddProductToManufacturerSearchModel> PrepareAddProductToManufacturerSearchModel(AddProductToManufacturerSearchModel model);
    }
    public class ManufacturerModelService : IManufacturerModelService
    {
        private readonly IBaseAdminModelService _baseAdminModelService;

        public ManufacturerModelService(IBaseAdminModelService baseAdminModelService)
        {
            _baseAdminModelService = baseAdminModelService;
        }

        public async Task<AddProductToManufacturerSearchModel> PrepareAddProductToManufacturerSearchModel(AddProductToManufacturerSearchModel model)
        {
            await _baseAdminModelService.PrepareSelectListCategoryAsync(model.AvailableCategories);
            await _baseAdminModelService.PrepareSelectListManufactureAsync(model.AvailableManufacturers);
            return model;
        }
    }
}
