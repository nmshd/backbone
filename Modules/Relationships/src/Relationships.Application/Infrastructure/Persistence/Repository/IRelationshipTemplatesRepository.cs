using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;

namespace Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;

public interface IRelationshipTemplatesRepository
{
    Task<DbPaginationResult<RelationshipTemplate>> FindTemplatesWithIds(IEnumerable<RelationshipTemplateId> ids, IdentityAddress identityAddress, PaginationFilter paginationFilter,
        CancellationToken cancellationToken, bool track = false);

    Task<RelationshipTemplate?> Find(RelationshipTemplateId id, IdentityAddress identityAddress, CancellationToken cancellationToken, bool track = false);
    Task Add(RelationshipTemplate template, CancellationToken cancellationToken);
    Task Update(RelationshipTemplate template);
    Task Delete(Expression<Func<RelationshipTemplate, bool>> filter, CancellationToken cancellationToken);

    #region RelationshipTemplateAllocations

    Task<IEnumerable<RelationshipTemplateAllocation>> FindRelationshipTemplateAllocations(Expression<Func<RelationshipTemplateAllocation, bool>> filter, CancellationToken cancellationToken);
    Task UpdateRelationshipTemplateAllocations(List<RelationshipTemplateAllocation> templateAllocations, CancellationToken cancellationToken);

    #endregion RelationshipTemplateAllocations

}
