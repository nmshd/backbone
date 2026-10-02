using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Quotas.Abstractions;

public interface IMessagesRepository
{
    Task<uint> Count(IdentityAddress sender, DateTime createdAtFrom, DateTime createdAtTo, CancellationToken cancellationToken);
}
