using Backbone.Modules.Relationships.Domain.Entities;
using Backbone.Modules.Relationships.Domain.Ids;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
public interface IRelationshipTemplatesRepository
{
    Task<DbPaginationResult<RelationshipTemplate>> FindTemplatesWithIds(IEnumerable<RelationshipTemplateId> ids, IdentityAddress identityAddress, PaginationFilter paginationFilter, bool track = false);
    Task<RelationshipTemplate> FindRelationshipTemplate(RelationshipTemplateId id, IdentityAddress identityAddress, CancellationToken cancellationToken, bool track = false, bool fillContent = true);
    Task<RelationshipTemplateId> AddRelationshipTemplate(RelationshipTemplate template, CancellationToken cancellationToken);
    Task UpdateRelationshipTemplate(RelationshipTemplate template);
}
