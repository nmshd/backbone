using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;

public interface IMessagesRepository
{
    Task<uint> Count(IdentityAddress sender, DateTime createdAtFrom, DateTime createdAtTo, CancellationToken cancellationToken);
}
