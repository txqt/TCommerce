using Microsoft.AspNetCore.Components.Authorization;
using NuGet.Protocol.Plugins;
using System.Net.Http.Headers;
using System.Text.Json;
using T.Library.Model;
using T.Library.Model.Account;
using T.Library.Model.Interface;
using T.Library.Model.RefreshToken;
using T.Library.Model.Response;

namespace T.Web.Services.UserRegistrationServices
{
    public interface IUserRegistrationService : IUserRegistrationServiceCommon
    {
        Task<ServiceResponse<AuthResponseDto>> Login(AccessTokenRequestModel loginRequest);
        Task Logout();
    }
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;

        public UserRegistrationService(HttpClient httpClient, JsonSerializerOptions options)
        {
            _httpClient = httpClient;
            _options = options;
        }

        public async Task<ServiceResponse<AuthResponseDto>> Login(AccessTokenRequestModel loginRequest)
        {
            var result = await _httpClient.PostAsJsonAsync("api/token/create", loginRequest);
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

        public async Task<ServiceResponse<string>> SendResetPasswordEmail(string email)
        {
            var result = await _httpClient.PostAsync($"/api/account/forgot-password?email={email}", null);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<string>>();
        }

        public async Task<ServiceResponse<string>> ResetPassword(ResetPasswordRequest resetPasswordRequest)
        {
            var result = await _httpClient.PostAsJsonAsync("/api/account/reset-password", resetPasswordRequest);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<string>>();
        }

        public Task<ServiceResponse<string>> ConfirmEmail(string userId, string token)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<string>> ChangePassword(ChangePasswordRequest model)
        {
            throw new NotImplementedException();
        }
    }
}
