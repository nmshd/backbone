using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.ListRelationshipTemplates;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;

namespace Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;

public interface IRelationshipTemplatesRepository
{
    Task<DbPaginationResult<RelationshipTemplate>> List(IEnumerable<ListRelationshipTemplatesQueryItem> queryItems, IdentityAddress activeIdentity, PaginationFilter paginationFilter,
        CancellationToken cancellationToken, bool track = false);

    Task<IEnumerable<RelationshipTemplate>> List(Expression<Func<RelationshipTemplate, bool>> filter, CancellationToken cancellationToken);

    Task<RelationshipTemplate?> Get(RelationshipTemplateId id, IdentityAddress identityAddress, CancellationToken cancellationToken, bool track = false);
    Task Add(RelationshipTemplate template, CancellationToken cancellationToken);
    Task Update(RelationshipTemplate template);
    Task Update(IEnumerable<RelationshipTemplate> templates, CancellationToken cancellationToken);
    Task Delete(Expression<Func<RelationshipTemplate, bool>> filter, CancellationToken cancellationToken);
    Task Delete(RelationshipTemplate template, CancellationToken cancellationToken);

    #region RelationshipTemplateAllocations

    Task<IEnumerable<RelationshipTemplateAllocation>> ListRelationshipTemplateAllocations(Expression<Func<RelationshipTemplateAllocation, bool>> filter, CancellationToken cancellationToken);

    Task<IEnumerable<TResult>> ListRelationshipTemplateAllocations<TResult>(
        Expression<Func<RelationshipTemplateAllocation, bool>> filter, Expression<Func<RelationshipTemplateAllocation, TResult>> selector, CancellationToken cancellationToken);

    Task UpdateRelationshipTemplateAllocations(List<RelationshipTemplateAllocation> templateAllocations, CancellationToken cancellationToken);

    #endregion RelationshipTemplateAllocations
}
