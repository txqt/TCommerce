using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using TCommerce.Core.Models.Discounts;
using TCommerce.Services.PrepareModelServices.PrepareAdminModel;
using TCommerce.Web.Areas.Admin.Models;
using TCommerce.Web.Areas.Admin.Models.SearchModel;

namespace TCommerce.Web.Areas.Admin.Services.PrepareAdminModel
{
    public interface IAdminDiscountModelService
    {
        Task<DiscountSearchModel> PrepareDiscountSearchModelModelAsync(DiscountSearchModel model);
        Task<DiscountModel> PrepareDiscountModelAsync(DiscountModel model, Discount discount);
        Task<DiscountProductSearchModel> PrepareAddProductToDiscountSearchModel(DiscountProductSearchModel model);
    }
    public class AdminDiscountModelService : IAdminDiscountModelService
    {
        private readonly IBaseAdminModelService _adminModelService;
        private readonly IMapper _mapper;

        public AdminDiscountModelService(IBaseAdminModelService adminModelService, IMapper mapper)
        {
            _adminModelService = adminModelService;
            _mapper = mapper;
        }

        public async Task<DiscountSearchModel> PrepareDiscountSearchModelModelAsync(DiscountSearchModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            await _adminModelService.PrepareSelectListDiscountTypeAsync(model.AvailableDiscountTypes);

            model.AvailableActiveOptions.Add(new SelectListItem
            {
                Value = "0",
                Text = "All"
            });
            model.AvailableActiveOptions.Add(new SelectListItem
            {
                Value = "1",
                Text = "Active only"
            });
            model.AvailableActiveOptions.Add(new SelectListItem
            {
                Value = "2",
                Text = "Inactive only"
            });

            return model;
        }

        public virtual async Task<DiscountModel> PrepareDiscountModelAsync(DiscountModel model, Discount discount)
        {
            if (discount != null)
            {
                model ??= _mapper.Map<DiscountModel>(discount);
            }

            if (discount == null)
            {
                model.IsActive = true;
                model.LimitationTimes = 1;
            }

            return model;
        }
        protected virtual DiscountProductSearchModel PrepareDiscountProductSearchModel(DiscountProductSearchModel searchModel, Discount discount)
        {
            ArgumentNullException.ThrowIfNull(searchModel);

            ArgumentNullException.ThrowIfNull(discount);

            return searchModel;
        }
        protected virtual DiscountCategorySearchModel PrepareDiscountCategorySearchModel(DiscountCategorySearchModel searchModel, Discount discount)
        {
            ArgumentNullException.ThrowIfNull(searchModel);

            ArgumentNullException.ThrowIfNull(discount);

            return searchModel;
        }
        protected virtual DiscountManufacturerSearchModel PrepareDiscountManufacturerSearchModel(DiscountManufacturerSearchModel searchModel,
        Discount discount)
        {
            ArgumentNullException.ThrowIfNull(searchModel);

            ArgumentNullException.ThrowIfNull(discount);

            return searchModel;
        }

        public async Task<DiscountProductSearchModel> PrepareAddProductToDiscountSearchModel(DiscountProductSearchModel model)
        {
            await _adminModelService.PrepareSelectListCategoryAsync(model.AvailableCategories);
            await _adminModelService.PrepareSelectListManufactureAsync(model.AvailableManufacturers);
            return model;
        }
    }
}
