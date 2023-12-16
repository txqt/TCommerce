using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using T.Library.Model.Roles.RoleName;

namespace T.Web.Helpers
{
    public class HttpClientHelper
    {
        private readonly HttpClient _httpClient;

        public HttpClientHelper(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void AddHeader(string name, string value)
        {
            if (_httpClient.DefaultRequestHeaders.Contains(name))
            {
                _httpClient.DefaultRequestHeaders.Remove(name);
            }
            _httpClient.DefaultRequestHeaders.Add(name, value);
        }

        public async Task<T> GetAsync<T>(string url)
        {
            var response = await _httpClient.GetAsync(url);
            return await HandleResponse<T>(response);
        }

        public async Task<T> PostAsJsonAsync<T>(string url, object data)
        {
            var response = await _httpClient.PostAsJsonAsync(url, data);
            return await HandleResponse<T>(response);
        }

        public async Task<T> PutAsJsonAsync<T>(string url, object data)
        {
            var response = await _httpClient.PutAsJsonAsync(url, data);
            return await HandleResponse<T>(response);
        }

        public async Task<T> DeleteAsync<T>(string url)
        {
            var response = await _httpClient.DeleteAsync(url);
            return await HandleResponse<T>(response);
        }

        public async Task<T> PostWithFormFileAsync<T>(string url, object data, IFormFile file = null)
        {
            var content = BuildMultipartFormDataContent(data, file);
            var response = await _httpClient.PostAsync(url, content);
            return await HandleResponse<T>(response);
        }

        public async Task<T> PutWithFormFileAsync<T>(string url, object data, IFormFile file = null)
        {
            var content = BuildMultipartFormDataContent(data, file);
            var response = await _httpClient.PutAsync(url, content);
            return await HandleResponse<T>(response);
        }

        private MultipartFormDataContent BuildMultipartFormDataContent(object data, IFormFile file = null)
        {
            var content = new MultipartFormDataContent();

            foreach (var prop in data.GetType().GetProperties())
            {
                var value = prop.GetValue(data);
                if (value != null)
                {
                    content.Add(new StringContent(value.ToString()), prop.Name);
                }
            }

            if (file != null && file.OpenReadStream() != null)
            {
                var streamContent = new StreamContent(file.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

                var fieldName = data.GetType().GetProperties().FirstOrDefault(p => p.PropertyType == typeof(IFormFile) && p.GetValue(data) == file)?.Name;

                content.Add(streamContent, fieldName ?? "file", file.FileName);
            }

            return content;
        }

        private async Task<T> HandleResponse<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }
            else
            {
                // Xử lý exception hoặc trả về giá trị mặc định tùy thuộc vào yêu cầu của bạn
                return default(T);
            }
        }
    }
}
