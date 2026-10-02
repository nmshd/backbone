using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Quotas.Abstractions;

public interface IRelationshipTemplatesRepository
{
    Task<uint> Count(IdentityAddress createdBy, DateTime createdAtFrom, DateTime createdAtTo, CancellationToken cancellationToken);
}
