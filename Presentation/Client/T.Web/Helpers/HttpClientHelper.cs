using System.Net.Http.Headers;

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
            var result = await _httpClient.GetAsync(url);
            return await result.Content.ReadFromJsonAsync<T>();
        }

        public async Task<T> PostAsJsonAsync<T>(string url, object data)
        {
            var result = await _httpClient.PostAsJsonAsync(url, data);
            return await result.Content.ReadFromJsonAsync<T>();
        }

        public async Task<T> PutAsJsonAsync<T>(string url, object data)
        {
            var result = await _httpClient.PutAsJsonAsync(url, data);
            return await result.Content.ReadFromJsonAsync<T>();
        }

        public async Task<T> DeleteAsync<T>(string url)
        {
            var result = await _httpClient.DeleteAsync(url);
            return await result.Content.ReadFromJsonAsync<T>();
        }
        public async Task<T> PostAsFormDataAsync<T>(string url, object data, IFormFile file = null)
        {
            var content = new MultipartFormDataContent();

            // Add data properties to the content
            foreach (var prop in data.GetType().GetProperties())
            {
                var value = prop.GetValue(data);
                if (value != null)
                {
                    content.Add(new StringContent(value.ToString()), prop.Name);
                }
            }

            // Add file to the content
            if (file != null)
            {
                var streamContent = new StreamContent(file.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                content.Add(streamContent, nameof(file), file.FileName);
            }

            var response = await _httpClient.PostAsync(url, content);
            return await response.Content.ReadFromJsonAsync<T>();
        }
        public async Task<T> PutAsFormDataAsync<T>(string url, object data, IFormFile file = null)
        {
            var content = new MultipartFormDataContent();

            // Add data properties to the content
            foreach (var prop in data.GetType().GetProperties())
            {
                var value = prop.GetValue(data);
                if (value != null)
                {
                    content.Add(new StringContent(value.ToString()), prop.Name);
                }
            }

            // Add file to the content
            if (file != null)
            {
                var streamContent = new StreamContent(file.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                content.Add(streamContent, nameof(file), file.FileName);
            }

            var response = await _httpClient.PutAsync(url, content);
            return await response.Content.ReadFromJsonAsync<T>();
        }
    }
}
