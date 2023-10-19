﻿using Microsoft.AspNetCore.Identity;
using T.Library.Model.Users;
using T.WebApi.Services.TokenHelpers;

namespace T.WebApi.Middleware.JwtHandlings
{
    public class CheckBanMiddleware
    {
        private readonly RequestDelegate _next;

        public CheckBanMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<User> userManager, ITokenService tokenService)
        {
            if (context.Request.Headers.ContainsKey("Authorization"))
            {
                string token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var principal = tokenService.GetPrincipalToken(token);
                var username = principal.Identity.Name; // this depends on the claims you added to the token
                var user = await userManager.FindByNameAsync(username);

                if (user != null && user.IsBanned)
                {
                    context.Response.StatusCode = 401; // Unauthorized
                    await context.Response.WriteAsync("Your account has been banned");
                    return;
                }
            }

            await _next(context);
        }
    }
}