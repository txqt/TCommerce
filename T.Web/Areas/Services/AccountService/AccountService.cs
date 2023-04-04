using Microsoft.AspNetCore.Components.Authorization;
using NuGet.Protocol.Plugins;
using System.Net.Http.Headers;
using System.Text.Json;
using T.Library.Model;
using T.Library.Model.RefreshToken;
using T.Library.Model.Response;

namespace T.Web.Areas.Services.AccountService
{
    public interface IAccountService
    {
        Task<ServiceResponse<AuthResponseDto>> Login(LoginViewModel loginRequest);
    }
    public class AccountService : IAccountService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;

        public AccountService(HttpClient httpClient, JsonSerializerOptions options)
        {
            _httpClient = httpClient;
            _options = options;
        }

        public async Task<ServiceResponse<AuthResponseDto>> Login(LoginViewModel loginRequest)
        {
            var result = await _httpClient.PostAsJsonAsync("api/account/login", loginRequest);
            var content = await result.Content.ReadAsStringAsync();
            var loginResponse = JsonSerializer.Deserialize<ServiceResponse<AuthResponseDto>>(content, _options);
            if (!result.IsSuccessStatusCode)
            {
                return loginResponse;
            }
            //await _localStorage.SetItemAsync("authToken", loginResponse.Data.Token);
            //await _localStorage.SetItemAsync("refreshToken", loginResponse.Data.RefreshToken);
            //((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(loginResponse.Data.AccessToken);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse.Data.AccessToken);
            return loginResponse;
        }
    }
}
