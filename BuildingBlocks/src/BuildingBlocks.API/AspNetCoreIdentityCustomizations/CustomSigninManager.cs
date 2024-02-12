using Backbone.Modules.Devices.Domain.Entities;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Backbone.BuildingBlocks.API.AspNetCoreIdentityCustomizations;

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
        {
            await FailedLogin(user);
            return result;
        }

        await UpdateLastLoginDate(user);

        return result;
    }

    private async Task UpdateLastLoginDate(ApplicationUser user)
    {
        user.LoginOccurred();
        await UserManager.UpdateAsync(user);
    }

    private async Task FailedLogin(ApplicationUser user)
    {
        if (!user.FirstOf3FailedAt.HasValue)
            user.FirstOf3FailedAt = DateTimeOffset.UtcNow;

        var firstOf3FailedAt = (DateTimeOffset)user.FirstOf3FailedAt;
        if (DateTimeOffset.Compare(firstOf3FailedAt.AddMinutes(3), DateTimeOffset.UtcNow) < 0)
        {
            user.AccessFailedCount = 1;
            user.FirstOf3FailedAt = DateTimeOffset.UtcNow;
        }

        await UserManager.UpdateAsync(user);
    }
}
