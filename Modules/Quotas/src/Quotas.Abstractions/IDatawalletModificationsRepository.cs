namespace Backbone.Modules.Quotas.Abstractions;

public interface IDatawalletModificationsRepository
{
    Task<uint> Count(string identityAddress, DateTime from, DateTime to, CancellationToken cancellationToken);
}
