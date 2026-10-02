namespace Backbone.Modules.Devices.Contracts;

public interface IIdentityStatusProvider
{
    Task<bool> IsActive(string address, CancellationToken cancellationToken);
}
