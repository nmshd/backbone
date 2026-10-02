namespace Backbone.Modules.Quotas.Abstractions;

public interface IDevicesRepository
{
    Task<uint> Count(string identityAddress, DateTime from, DateTime to, CancellationToken cancellationToken);
}
