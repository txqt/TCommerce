using TCommerce.Services.PrepareModelServices.PrepareAdminModel;
using TCommerce.Core.Models.Catalogs;
using AutoMapper;
using TCommerce.Core.Interface;
using TCommerce.Web.Areas.Admin.Models.Catalog;

namespace TCommerce.Web.Areas.Admin.Services.PrepareAdminModel
{
    public interface IAdminManufacturerModelService
    {
        Task<ProductManufacturerSearchModel> PrepareAddProductToManufacturerSearchModel(ProductManufacturerSearchModel model);

        Task<ManufacturerModel> PrepareManufacturerModelAsync(ManufacturerModel model, Manufacturer manufacturer);
    }
    public class AdminManufacturerModelService : IAdminManufacturerModelService
    {
        private readonly IBaseAdminModelService _baseAdminModelService;
        private readonly IMapper _mapper;
        private readonly IUrlRecordService _urlRecordService;

        public AdminManufacturerModelService(IBaseAdminModelService baseAdminModelService, IMapper mapper, IUrlRecordService urlRecordService)
        {
            _baseAdminModelService = baseAdminModelService;
            _mapper = mapper;
            _urlRecordService = urlRecordService;
        }

        public async Task<ProductManufacturerSearchModel> PrepareAddProductToManufacturerSearchModel(ProductManufacturerSearchModel model)
        {
            await _baseAdminModelService.PrepareSelectListCategoryAsync(model.AvailableCategories);
            await _baseAdminModelService.PrepareSelectListManufactureAsync(model.AvailableManufacturers);
            return model;
        }

        public async Task<ManufacturerModel> PrepareManufacturerModelAsync(ManufacturerModel model, Manufacturer manufacturer)
        {
            if(manufacturer is not null)
            {
                model ??= new ManufacturerModel()
                {
                    Id = manufacturer.Id
                };
                _mapper.Map(manufacturer, model);
                model.SeName = await _urlRecordService.GetSeNameAsync(manufacturer);
            }
            return model;
        }
    }
}
