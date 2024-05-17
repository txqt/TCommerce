using Microsoft.AspNetCore.Mvc.Rendering;
using T.Web.Services.AddressServices;

namespace T.Web.Services.PrepareModelServices
{
    public interface IBaseModelService
    {
        Task PrepareSelectListProvinceAsync(List<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);
        Task PrepareSelectListDistrictAsync(List<SelectListItem> items, int provinceId, bool withSpecialDefaultItem = true, string defaultItemText = null);
        Task PrepareSelectListCommuneAsync(List<SelectListItem> items, int districtId, bool withSpecialDefaultItem = true, string defaultItemText = null);
    }
    public class BaseModelService : IBaseModelService
    {
        private readonly IAddressService _addressService;

        public BaseModelService(IAddressService addressService)
        {
            _addressService = addressService;
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

        public async Task PrepareSelectListProvinceAsync(List<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            ArgumentNullException.ThrowIfNull(items);

            //prepare available manufacturers
            var availableProvinceItems = await GetProvinceListAsync();
            foreach (var provinceItem in availableProvinceItems)
            {
                items.Add(provinceItem);
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        public async Task PrepareSelectListDistrictAsync(List<SelectListItem> items, int provinceId, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            ArgumentNullException.ThrowIfNull(items);

            //prepare available manufacturers
            var availableItems = await GetDistrictListByProvinceIdAsync(provinceId);
            foreach (var item in availableItems)
            {
                items.Add(item);
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        public async Task PrepareSelectListCommuneAsync(List<SelectListItem> items, int districtId, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            ArgumentNullException.ThrowIfNull(items);

            //prepare available manufacturers
            var availableItems = await GetCommuneListByDistrictIdAsync(districtId);
            foreach (var item in availableItems)
            {
                items.Add(item);
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }
        protected virtual async Task<List<SelectListItem>> GetProvinceListAsync()
        {
            var provinces = await _addressService.GetAllProvinces();

            var listItems = provinces.Select(m => new SelectListItem
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
        protected virtual async Task<List<SelectListItem>> GetDistrictListByProvinceIdAsync(int provinceId)
        {
            var districts = await _addressService.GetDistrictsByProvinceId(provinceId);

            var listItems = districts.Select(m => new SelectListItem
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
        protected virtual async Task<List<SelectListItem>> GetCommuneListByDistrictIdAsync(int districtId)
        {
            var communes = await _addressService.GetCommunesByDistrictId(districtId);

            var listItems = communes.Select(m => new SelectListItem
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
