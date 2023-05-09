﻿using Backbone.Modules.Devices.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ConsumerApi.AspNetCoreIdentityCustomizations;

public class CustomSigninManager : SignInManager<ApplicationUser>
{
    public CustomSigninManager(
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor contextAccessor,
        IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
        IOptions<IdentityOptions> optionsAccessor,
        ILogger<SignInManager<ApplicationUser>> logger,
        IAuthenticationSchemeProvider schemes,
        IUserConfirmation<ApplicationUser> confirmation) : base(userManager, contextAccessor, claimsFactory,
        optionsAccessor, logger, schemes, confirmation)
    { }

    public override async Task<SignInResult> CheckPasswordSignInAsync(ApplicationUser user, string password,
        bool lockoutOnFailure)
    {
        var result = await base.CheckPasswordSignInAsync(user, password, lockoutOnFailure);

        if (!result.Succeeded)
            return result;

        await UpdateLastLoginDate(user);

        return result;
    }

    private async Task UpdateLastLoginDate(ApplicationUser user)
    {
        user.LoginOccurred();
        await UserManager.UpdateAsync(user);
    }
}
