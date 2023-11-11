using T.Library.Model.Response;
using T.Library.Model;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using T.Library.Model.ViewsModel;
using System.Net.Http.Headers;
using T.Library.Model.Security;
using T.Library.Model.Interface;
using T.Library.Model.Users;

namespace T.Web.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ServiceResponse<bool>> CreateUserAsync(UserModel model)
        {
            var result = await _httpClient.PostAsJsonAsync($"api/user", model);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }
        public async Task<ServiceResponse<bool>> UpdateUserAsync(UserModel model)
        {
            var result = await _httpClient.PutAsJsonAsync($"api/user", model);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<bool>> DeleteUserByUserIdAsync(Guid id)
        {
            var result = await _httpClient.DeleteAsync($"api/user/{id}");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<UserModel>> Get(Guid id)
        {
            var result = await _httpClient.GetAsync($"api/user/{id}");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<UserModel>>();
        }

        public async Task<List<UserModel>> GetAllAsync()
        {
            var result = await _httpClient.GetAsync($"api/user/{APIRoutes.GETALL}");
            return await result.Content.ReadFromJsonAsync<List<UserModel>>();
        }

        public Task<List<Role>> GetRolesByUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<User>> GetCurrentUser()
        {
            throw new NotImplementedException();
        }


        public Task<ServiceResponse<bool>> BanUser(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
