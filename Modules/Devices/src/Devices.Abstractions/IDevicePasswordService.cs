using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Abstractions;

public interface IDevicePasswordService
{
    Task<string?> ChangePassword(ApplicationUser user, string oldPassword, string newPassword);
}
