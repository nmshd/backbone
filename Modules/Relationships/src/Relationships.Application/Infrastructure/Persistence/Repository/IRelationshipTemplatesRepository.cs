using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Relationships.Domain.Entities;
using Backbone.Relationships.Domain.Ids;

namespace Backbone.Relationships.Application.Infrastructure.Persistence.Repository;
public interface IRelationshipTemplatesRepository
{
    Task<DbPaginationResult<RelationshipTemplate>> FindTemplatesWithIds(IEnumerable<RelationshipTemplateId> ids, IdentityAddress identityAddress, PaginationFilter paginationFilter, CancellationToken cancellationToken, bool track = false);
    Task<RelationshipTemplate> Find(RelationshipTemplateId id, IdentityAddress identityAddress, CancellationToken cancellationToken, bool track = false, bool fillContent = true);
    Task Add(RelationshipTemplate template, CancellationToken cancellationToken);
    Task Update(RelationshipTemplate template);
}
