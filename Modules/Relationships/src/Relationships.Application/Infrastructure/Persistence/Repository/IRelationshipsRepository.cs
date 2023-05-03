using Backbone.Modules.Relationships.Domain.Entities;
using Backbone.Modules.Relationships.Domain.Ids;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
public interface IRelationshipsRepository
{
    Task<RelationshipTemplate> FindRelationshipTemplate(RelationshipTemplateId id, IdentityAddress identityAddress, CancellationToken cancellationToken, bool track = false, bool fillContent = true);
    Task<RelationshipTemplateId> AddRelationshipTemplate(RelationshipTemplate template, CancellationToken cancellationToken);
    Task UpdateRelationshipTemplate(RelationshipTemplate template);
}
