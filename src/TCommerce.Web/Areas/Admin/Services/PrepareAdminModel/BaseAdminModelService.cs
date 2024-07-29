using Microsoft.AspNetCore.Mvc.Rendering;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Discounts;
using TCommerce.Core.Models.Orders;
using TCommerce.Core.Models.Payments;
using TCommerce.Web.Extensions;

namespace TCommerce.Services.PrepareModelServices.PrepareAdminModel
{
    public interface IBaseAdminModelService
    {
        Task PrepareSelectListCategoryAsync(List<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);
        Task PrepareSelectListManufactureAsync(List<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);
        Task PrepareSelectListDiscountTypeAsync(List<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);
        Task PrepareOrderStatusesAsync(List<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);
        Task PreparePaymentStatusesAsync(List<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);
    }
    public class BaseAdminModelService : IBaseAdminModelService
    {
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;

        public BaseAdminModelService(ICategoryService categoryService, IManufacturerService manufacturerService)
        {
            _categoryService = categoryService;
            _manufacturerService = manufacturerService;
        }

        protected void PrepareDefaultItem(List<SelectListItem> items, bool withSpecialDefaultItem, string defaultItemText = null, string defaultItemValue = "0")
        {
            ArgumentNullException.ThrowIfNull(items);

            //whether to insert the first special item for the default value
            if (!withSpecialDefaultItem)
                return;

            //prepare item text
            defaultItemText ??= "All";

            //insert this default item at first
            items.Insert(0, new SelectListItem { Text = defaultItemText, Value = defaultItemValue });
        }

        public async Task PrepareSelectListCategoryAsync(List<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            ArgumentNullException.ThrowIfNull(items);

            //prepare available manufacturers
            var availableManufacturerItems = await GetCategoryListAsync();
            foreach (var manufacturerItem in availableManufacturerItems)
            {
                items.Add(manufacturerItem);
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        public async Task PrepareSelectListManufactureAsync(List<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            ArgumentNullException.ThrowIfNull(items);

            //prepare available manufacturers
            var availableManufacturerItems = await GetManufacturerListAsync();
            foreach (var manufacturerItem in availableManufacturerItems)
            {
                items.Add(manufacturerItem);
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }
        protected virtual async Task<List<SelectListItem>> GetManufacturerListAsync()
        {
            var manufacturers = await _manufacturerService.GetAllManufacturerAsync();

            if (manufacturers == null)
            {
                return new List<SelectListItem>();
            }

            var listItems = manufacturers.Select(m => new SelectListItem
            {
                Text = m.Name,
                Value = m.Id.ToString()
            }).ToList();

            var result = new List<SelectListItem>();
            // Clone the list to ensure that "selected" property is not set
            foreach (var item in listItems)
            {
                result.Add(new SelectListItem
                {
                    Text = item.Text,
                    Value = item.Value
                });
            }

            return result;
        }

        protected virtual async Task<List<SelectListItem>> GetCategoryListAsync()
        {
            var categories = await _categoryService.GetAllCategoryAsync();

            if (categories == null)
            {
                return new List<SelectListItem>();
            }

            var listItems = categories.Select(m => new SelectListItem
            {
                Text = m.Name,
                Value = m.Id.ToString()
            }).ToList();

            var result = new List<SelectListItem>();
            // Clone the list to ensure that "selected" property is not set
            foreach (var item in listItems)
            {
                result.Add(new SelectListItem
                {
                    Text = item.Text,
                    Value = item.Value
                });
            }

            return result;
        }

        public async Task PrepareSelectListDiscountTypeAsync(List<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            ArgumentNullException.ThrowIfNull(items);

            //prepare available manufacturers
            var availableDiscountItems = TEnumExtensions.ToSelectList<DiscountType>();
            foreach (var discountItem in availableDiscountItems)
            {
                items.Add(discountItem);
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        public async Task PrepareOrderStatusesAsync(List<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            ArgumentNullException.ThrowIfNull(items);

            //prepare available order statuses
            var availableStatusItems = TEnumExtensions.ToSelectList<OrderStatus>();
            foreach (var statusItem in availableStatusItems)
            {
                items.Add(statusItem);
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        public async Task PreparePaymentStatusesAsync(List<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            ArgumentNullException.ThrowIfNull(items);

            //prepare available payment statuses
            var availableStatusItems = TEnumExtensions.ToSelectList<PaymentStatus>();
            foreach (var statusItem in availableStatusItems)
            {
                items.Add(statusItem);
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }
    }
}
