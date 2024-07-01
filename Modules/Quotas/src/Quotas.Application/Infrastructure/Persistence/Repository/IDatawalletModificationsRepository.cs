namespace Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;

public interface IDatawalletModificationsRepository
{
    Task<uint> Count(string identityAddress, DateTime from, DateTime to, CancellationToken cancellationToken);
}
