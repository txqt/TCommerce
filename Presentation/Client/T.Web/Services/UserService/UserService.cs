﻿using T.Library.Model.Response;
using T.Library.Model;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using T.Library.Model.ViewsModel;
using System.Net.Http.Headers;
using T.Library.Model.Security;
using T.Library.Model.Interface;
using T.Library.Model.Users;
using T.Library.Model.RefreshToken;
using T.Library.Model.Account;
using Microsoft.AspNetCore.Mvc;
using T.Web.Helpers;

namespace T.Web.Services.UserService
{
    public interface IUserService : IUserServiceCommon
    {
        Task<ServiceResponse<bool>> Register(RegisterRequest request);
    }
    public class UserService : HttpClientHelper, IUserService
    {
        public UserService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<ServiceResponse<bool>> CreateUserAsync(UserModel model)
        {
            return await PostAsJsonAsync<ServiceResponse<bool>>($"api/user", model);
        }
        public async Task<ServiceResponse<bool>> UpdateUserAsync(UserModel model)
        {
            return await PutAsJsonAsync<ServiceResponse<bool>>($"api/user", model);
        }

        public async Task<ServiceResponse<bool>> DeleteUserByUserIdAsync(Guid id)
        {
            return await DeleteAsync<ServiceResponse<bool>>($"api/user/{id}");
        }

        public async Task<UserModel> Get(Guid id)
        {
            return await GetAsync<UserModel>($"api/user/{id}");
        }

        public async Task<List<UserModel>> GetAllAsync()
        {
            return await GetAsync<List<UserModel>>($"api/user/{APIRoutes.GETALL}");
        }

        public Task<List<Role>> GetRolesByUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetCurrentUser()
        {
            throw new NotImplementedException();
        }


        public Task<ServiceResponse<bool>> BanUser(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Logout(Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<bool>> Register(RegisterRequest registerRequest)
        {
            return await PostAsJsonAsync<ServiceResponse<bool>>("api/user/account/register", registerRequest);
        }
    }
}
