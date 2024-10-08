﻿using Microsoft.AspNetCore.Identity;

namespace TCommerce.Web.IdentityCustoms
{
    public class CustomPasswordValidator<TUser> : IPasswordValidator<TUser> where TUser : class
    {
        public async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string? password)
        {
            var username = await manager.GetUserNameAsync(user);
            if (username is not null && username.ToLower().Equals(password?.ToLower()))
                return IdentityResult.Failed(new IdentityError { Description = "Username and Password can't be the same.", Code = "SameUserPass" });

            if (password?.ToLower().Contains("password") == true)
                return IdentityResult.Failed(new IdentityError { Description = "The word password is not allowed for the Password.", Code = "PasswordContainsPassword" });

            return IdentityResult.Success;
        }
    }
}
