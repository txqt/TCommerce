using Microsoft.AspNetCore.Components.Authorization;
using NuGet.Protocol.Plugins;
using System.Net.Http.Headers;
using System.Text.Json;
using T.Library.Model;
using T.Library.Model.Account;
using T.Library.Model.RefreshToken;
using T.Library.Model.Response;

namespace T.Web.Services.AccountService
{
    public interface IAccountService
    {
        Task<ServiceResponse<AuthResponseDto>> Login(LoginViewModel loginRequest);
        Task Logout();
        Task<ServiceResponse<bool>> Register(RegisterRequest registerRequest);
        Task<ServiceResponse<string>> ForgotPassword(ForgotPasswordViewModel forgotPasswordViewModel);
        Task<ServiceResponse<string>> ResetPassword(ResetPasswordRequest resetPasswordRequest);
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
            _httpClient.DefaultRequestHeaders.Authorization = null;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse.Data.AccessToken);
            return loginResponse;
        }

        public async Task Logout()
        {
            await _httpClient.PostAsync("api/account/logout", null);
        }

        public async Task<ServiceResponse<bool>> Register(RegisterRequest registerRequest)
        {
            var result = await _httpClient.PostAsJsonAsync("api/account/register", registerRequest);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<string>> ForgotPassword(ForgotPasswordViewModel forgotPasswordViewModel)
        {
            var result = await _httpClient.PostAsync($"/api/account/forgot-password?email={forgotPasswordViewModel.Email}", null);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<string>>();
        }

        public async Task<ServiceResponse<string>> ResetPassword(ResetPasswordRequest resetPasswordRequest)
        {
            var result = await _httpClient.PostAsJsonAsync("/api/account/reset-password", resetPasswordRequest);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<string>>();
        }
    }
}
