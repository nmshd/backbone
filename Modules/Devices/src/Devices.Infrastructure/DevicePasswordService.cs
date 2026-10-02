using Backbone.Modules.Devices.Abstractions;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Microsoft.AspNetCore.Identity;

namespace Backbone.Modules.Devices.Infrastructure;

public class DevicePasswordService(UserManager<ApplicationUser> userManager) : IDevicePasswordService
{
    public async Task<string?> ChangePassword(ApplicationUser user, string oldPassword, string newPassword)
    {
        var result = await userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        return result.Succeeded ? null : result.Errors.First().Description;
    }
}
