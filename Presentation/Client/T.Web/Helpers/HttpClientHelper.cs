
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.Json;
using System;
using Newtonsoft.Json;
using T.Library.Model.Response;

namespace T.Web.Helpers
{
    public class HttpClientHelper
    {
        private readonly HttpClient _httpClient;
        private HttpResponseMessage _lastResponse;
        public HttpResponseMessage LastResponse => _lastResponse;

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

        public async Task<T> GetAsync<T>(string url, JsonSerializerOptions jsonOptions = null)
        {
            var response = await _httpClient.GetAsync(url);
            _lastResponse = response;
            return await HandleResponse<T>(response, jsonOptions);
        }

        public async Task<T> PostAsJsonAsync<T>(string url, object data, JsonSerializerOptions jsonOptions = null)
        {
            var response = await _httpClient.PostAsJsonAsync(url, data, jsonOptions);
            _lastResponse = response;
            return await HandleResponse<T>(response, jsonOptions);
        }

        public async Task<T> PutAsJsonAsync<T>(string url, object data, JsonSerializerOptions jsonOptions = null)
        {
            var response = await _httpClient.PutAsJsonAsync(url, data, jsonOptions);
            _lastResponse = response;
            return await HandleResponse<T>(response, jsonOptions);
        }

        public async Task<T> DeleteAsync<T>(string url, JsonSerializerOptions jsonOptions = null)
        {
            var response = await _httpClient.DeleteAsync(url);
            _lastResponse = response;
            return await HandleResponse<T>(response, jsonOptions);
        }

        public async Task<T> DeleteWithDataAsync<T>(string url, object data, JsonSerializerOptions jsonOptions = null)
        {
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(_httpClient.BaseAddress, url),
                Content = content
            });

            _lastResponse = response;
            return await HandleResponse<T>(response, jsonOptions);
        }


        public async Task<T> PostWithFormFileAsync<T>(string url, object data, IFormFile file = null, JsonSerializerOptions jsonOptions = null)
        {
            var content = BuildMultipartFormDataContent(data, file);
            var response = await _httpClient.PostAsync(url, content);
            _lastResponse = response;
            return await HandleResponse<T>(response, jsonOptions);
        }

        public async Task<T> PutWithFormFileAsync<T>(string url, object data, IFormFile file = null, JsonSerializerOptions jsonOptions = null)
        {
            var content = BuildMultipartFormDataContent(data, file);
            var response = await _httpClient.PutAsync(url, content);
            _lastResponse = response;
            return await HandleResponse<T>(response, jsonOptions);
        }
        public async Task<T> PostWithFormFilesAsync<T>(string url, object data, List<IFormFile> files = null, JsonSerializerOptions jsonOptions = null)
        {
            var content = new MultipartFormDataContent();
            foreach (var file in files)
            {
                content = BuildMultipartFormDataContent(data, file);
            }

            var response = await _httpClient.PostAsync(url, content);
            _lastResponse = response;
            return await HandleResponse<T>(response);
        }

        public async Task<T> PutWithFormFilesAsync<T>(string url, object data, List<IFormFile> files = null, JsonSerializerOptions jsonOptions = null)
        {
            var content = new MultipartFormDataContent();
            foreach (var file in files)
            {
                content = BuildMultipartFormDataContent(data, file);
            }
            var response = await _httpClient.PutAsync(url, content);
            _lastResponse = response;
            return await HandleResponse<T>(response);
        }

        public async Task<T> PutWithFormFileAsync<T>(string url, List<IFormFile> files = null, string apiParameterName = "formFiles", JsonSerializerOptions jsonOptions = null)
        {
            using var content = new MultipartFormDataContent();

            foreach (var file in files)
            {
                var fileContent = new StreamContent(file.OpenReadStream());
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = apiParameterName, // Đổi tên tham số API ở đây
                    FileName = file.FileName
                };
                content.Add(fileContent);
            }

            try
            {
                var response = await _httpClient.PostAsync(url, content);
                _lastResponse = response;
                return await HandleResponse<T>(response);
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ (ghi log, ném lại ngoại lệ, v.v.)
                Console.WriteLine($"Đã xảy ra lỗi: {ex.Message}");
                throw;
            }
        }


        private MultipartFormDataContent BuildMultipartFormDataContent(object data, IFormFile file = null)
        {
            var content = new MultipartFormDataContent();

            if(data != null)
            {
                foreach (var prop in data.GetType().GetProperties())
                {
                    var value = prop.GetValue(data);
                    if (value != null)
                    {
                        content.Add(new StringContent(value.ToString()), prop.Name);
                    }
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

        private async Task<T> HandleResponse<T>(HttpResponseMessage response, JsonSerializerOptions jsonOptions = null)
        {
            if (response.IsSuccessStatusCode)
            {
                if (typeof(T) == typeof(string))
                {
                    // Đọc và trả về nội dung dưới dạng chuỗi
                    return (T)(object)await response.Content.ReadAsStringAsync();
                }
                else if (response.Content.Headers.ContentLength.HasValue && response.Content.Headers.ContentLength.Value > 0)
                {
                    // Đọc và chuyển đổi nội dung JSON thành đối tượng kiểu T
                    return await response.Content.ReadFromJsonAsync<T>(jsonOptions);
                }
            }
            else if(response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return await response.Content.ReadFromJsonAsync<T>(jsonOptions);
            }
            // Xử lý exception hoặc trả về giá trị mặc định
            return default(T);
        }
    }
}
