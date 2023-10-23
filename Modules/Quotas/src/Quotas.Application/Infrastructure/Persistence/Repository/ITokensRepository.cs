using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Quotas.Application.Infrastructure.Persistence.Repository;
public interface ITokensRepository
{
    Task<uint> Count(IdentityAddress createdBy, DateTime createdAtFrom, DateTime createdAtTo, CancellationToken cancellationToken);
}
