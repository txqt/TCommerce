using T.Library.Model.Response;
using T.Library.Model;
using T.Library.Model.Users;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using T.Library.Model.ViewsModel;
using System.Net.Http.Headers;

namespace T.Web.Services.UserService
{
    public interface IUserService
    {
        Task<List<UserModel>> GetAllAsync();
        Task<List<Role>> GetAllRolesAsync();
        Task<ServiceResponse<UserModel>> Get(Guid id);
        Task<ServiceResponse<bool>> CreateOrEditAsync(UserModel model);
        Task<ServiceResponse<bool>> DeleteAsync(int id);
    }
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            var accessToken = _httpContextAccessor.HttpContext.Session.GetString("jwt");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        public async Task<ServiceResponse<bool>> CreateOrEditAsync(UserModel model)
        {
            var result = await _httpClient.PostAsJsonAsync($"api/user/{APIRoutes.AddOrEdit}", model);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<bool>> DeleteAsync(int id)
        {
            var result = await _httpClient.DeleteAsync($"api/user/delete/{id}");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<UserModel>> Get(Guid id)
        {
            var result = await _httpClient.GetAsync($"api/user/{id}");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<UserModel>>();
        }

        public async Task<List<UserModel>> GetAllAsync()
        {
            var result = await _httpClient.GetAsync($"api/user/{APIRoutes.GetAll}");
            return await result.Content.ReadFromJsonAsync<List<UserModel>>();
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            var result = await _httpClient.GetAsync($"api/user/all-roles");
            return await result.Content.ReadFromJsonAsync<List<Role>>();
        }
    }
}
