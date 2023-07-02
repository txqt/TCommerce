using T.Library.Model.Response;
using System.Net.Http.Json;
using T.Library.Model.Common;
using System.Net.Http.Headers;
using T.Library.Model.BannerItem;
using T.Library.Model.Slider;

namespace T.Web.Services.SliderItemService
{
    public interface ISliderItemService
    {
        Task<List<SliderItem>> GetAllSliderItemAsync();
        Task<ServiceResponse<bool>> CreateAsync(SaveSliderItemRequest request);
        Task<ServiceResponse<bool>> DeleteAllSliderItemAsync();
    }
    public class SliderItemService : ISliderItemService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SliderItemService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            var accessToken = _httpContextAccessor.HttpContext.Session.GetString("jwt");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        public async Task<ServiceResponse<bool>> CreateAsync(SaveSliderItemRequest request)
        {
            var result = await _httpClient.PostAsJsonAsync($"api/slider-item/create", request);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<bool>> DeleteAllSliderItemAsync()
        {
            var result = await _httpClient.DeleteAsync($"api/slider-item/delete");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<List<SliderItem>> GetAllSliderItemAsync()
        {
            var result = await _httpClient.GetAsync($"api/slider-item/{APIRoutes.GetAll}");
            return await result.Content.ReadFromJsonAsync<List<SliderItem>>();
        }
    }
}
