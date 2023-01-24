using System.Security.Claims;
using Devices.API.Models;
using Devices.Domain.Entities;
using Enmeshed.Tooling;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;

namespace Devices.API.AspNetCoreIdentityCustomizations;

public class ProfileService : IProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var principal = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));
        var userId = principal.Claims.First(x => x.Type == "sub").Value;

        var claims = await GetClaimsOfUser(userId);

        context.IssuedClaims = claims.ToList();
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));

        var subjectId = subject.Claims.First(x => x.Type == "sub").Value;
        var user = await _userManager.FindByIdAsync(subjectId);

        context.IsActive = false;

        if (user == null)
            return;

        if (_userManager.SupportsUserSecurityStamp)
        {
            var securityStamp = subject.Claims.SingleOrDefault(c => c.Type == "security_stamp")?.Value;
            if (securityStamp != null)
            {
                var dbSecurityStamp = await _userManager.GetSecurityStampAsync(user);
                if (dbSecurityStamp != securityStamp)
                    return;
            }
        }

        context.IsActive =
            !user.LockoutEnabled ||
            !user.LockoutEnd.HasValue ||
            user.LockoutEnd <= SystemTime.UtcNow;
    }

    private async Task<IEnumerable<Claim>> GetClaimsOfUser(string userId)
    {
        var user = await FindUserById(userId);

        var claims = new List<Claim>
        {
            new(JwtClaimTypes.Subject, user.Id),
            new(CustomClaims.ADDRESS, user.Device.Identity.Address),
            new(CustomClaims.DEVICE_ID, user.Device.Id)
        };

        var rolesOfUser = await _userManager.GetRolesAsync(user);
        claims.AddRange(rolesOfUser.Select(role => new Claim(JwtClaimTypes.Role, role)));

        return claims;
    }

    private async Task<ApplicationUser> FindUserById(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new ArgumentException("Invalid subject identifier");

        return user;
    }
}
