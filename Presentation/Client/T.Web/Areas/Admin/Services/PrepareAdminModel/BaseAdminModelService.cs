using Microsoft.AspNetCore.Mvc.Rendering;
using T.Library.Model.Interface;
using T.Web.Services.ManufacturerServices;

namespace T.Web.Services.PrepareModelServices.PrepareAdminModel
{
    public interface IBaseAdminModelService
    {
        Task PrepareSelectListCategoryAsync(List<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);
        Task PrepareSelectListManufactureAsync(List<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);
    }
    public class BaseAdminModelService : IBaseAdminModelService
    {
        private readonly ICategoryServiceCommon _categoryService;
        private readonly IManufacturerService _manufacturerService;

        public BaseAdminModelService(ICategoryServiceCommon categoryService, IManufacturerService manufacturerService)
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

            var listItems = manufacturers.Select(m => new SelectListItem
            {
                Text = m.Name,
                Value = m.Id.ToString()
            }).ToList();

            var result = new List<SelectListItem>();
            //clone the list to ensure that "selected" property is not set
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

            var listItems = categories.Select(m => new SelectListItem
            {
                Text = m.Name,
                Value = m.Id.ToString()
            }).ToList();

            var result = new List<SelectListItem>();
            //clone the list to ensure that "selected" property is not set
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
    }
}
