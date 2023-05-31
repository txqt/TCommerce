using T.Library.Model.Response;
using T.Library.Model;
using T.Library.Model.Users;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using T.Library.Model.ViewsModel;

namespace T.Web.Services.UserService
{
    public interface IUserService
    {
        Task<List<UserModel>> GetAllAsync();
        Task<ServiceResponse<UserModel>> Get(int id);
        Task<ServiceResponse<bool>> CreateOrEditAsync(UserModel model);
        Task<ServiceResponse<bool>> DeleteAsync(int id);
    }
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ServiceResponse<bool>> CreateOrEditAsync(UserModel model)
        {
            var result = await _httpClient.PostAsJsonAsync($"api/category/{APIRoutes.AddOrEdit}", model);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<bool>> DeleteAsync(int id)
        {
            var result = await _httpClient.DeleteAsync($"api/category/delete/{id}");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<UserModel>> Get(int id)
        {
            var result = await _httpClient.GetAsync($"api/category/{id}");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<UserModel>>();
        }

        public async Task<List<UserModel>> GetAllAsync()
        {
            var result = await _httpClient.GetAsync($"api/category/{APIRoutes.GetAll}");
            return await result.Content.ReadFromJsonAsync<List<UserModel>>();
        }
    }
}
