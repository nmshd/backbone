using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;

public interface IRelationshipsRepository
{
    Task<DbPaginationResult<Relationship>> ListRelationshipsWithContent(IEnumerable<RelationshipId> ids, IdentityAddress identityAddress, PaginationFilter paginationFilter,
        CancellationToken cancellationToken, bool track = false);

    Task<Relationship> GetRelationshipWithoutContent(RelationshipId id, IdentityAddress identityAddress, CancellationToken cancellationToken, bool track = false);
    Task<Relationship> GetRelationshipWithContent(RelationshipId id, IdentityAddress identityAddress, CancellationToken cancellationToken, bool track = false);
    Task<IdentityAddress> GetRelationshipPeer(RelationshipId id, IdentityAddress identityAddress, CancellationToken cancellationToken);

    Task Add(Relationship relationship, CancellationToken cancellationToken);
    Task Update(Relationship relationship);
    Task Update(IEnumerable<Relationship> relationships);
    Task<IEnumerable<Relationship>> ListWithoutContent(Expression<Func<Relationship, bool>> filter, CancellationToken cancellationToken, bool track = false, bool ignoreCache = false);

    Task<IEnumerable<T>> ListWithoutContent<T>(Expression<Func<Relationship, bool>> filter, Expression<Func<Relationship, T>> selector, CancellationToken cancellationToken,
        bool track = false);

    Task<int> Delete(Expression<Func<Relationship, bool>> filter, CancellationToken cancellationToken);
}
