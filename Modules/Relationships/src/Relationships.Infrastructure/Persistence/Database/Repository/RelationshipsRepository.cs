using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Application.Infrastructure;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Infrastructure.Extensions;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Relationships.Infrastructure.Persistence.Database.Repository;

public class RelationshipsRepository : IRelationshipsRepository
{
    private readonly DbSet<Relationship> _relationships;
    private readonly IQueryable<Relationship> _readOnlyRelationships;
    private readonly RelationshipsDbContext _dbContext;

    public RelationshipsRepository(RelationshipsDbContext dbContext)
    {
        _relationships = dbContext.Relationships;
        _readOnlyRelationships = dbContext.Relationships.AsNoTracking();
        _dbContext = dbContext;
    }

    public async Task<IdentityAddress> GetRelationshipPeer(RelationshipId id, IdentityAddress identityAddress, CancellationToken cancellationToken)
    {
        var relationship = await _readOnlyRelationships
            .WithParticipant(identityAddress)
            .FirstWithId(id, cancellationToken);

        return relationship.GetPeerOf(identityAddress);
    }

    public async Task<Relationship> GetRelationshipWithContent(RelationshipId id, IdentityAddress identityAddress, CancellationToken cancellationToken, bool track = false)
    {
        var relationship = await (track ? _relationships : _readOnlyRelationships)
            .IncludeAll(_dbContext)
            .AsSplitQuery()
            .WithParticipant(identityAddress)
            .FirstWithId(id, cancellationToken);

        return relationship;
    }

    public async Task<Relationship> GetRelationshipWithoutContent(RelationshipId id, IdentityAddress identityAddress, CancellationToken cancellationToken, bool track = false)
    {
        var relationship = await (track ? _relationships : _readOnlyRelationships)
            .Include(r => r.RelationshipTemplate)
            .Include(r => r.AuditLog)
            .AsSplitQuery()
            .WithParticipant(identityAddress)
            .FirstWithId(id, cancellationToken);

        return relationship;
    }

    public async Task<DbPaginationResult<Relationship>> ListRelationshipsWithContent(IEnumerable<RelationshipId> ids,
        IdentityAddress identityAddress, PaginationFilter paginationFilter, CancellationToken cancellationToken, bool track = false)
    {
        var query = (track ? _relationships : _readOnlyRelationships)
            .AsQueryable()
            .IncludeAll(_dbContext)
            .AsSplitQuery()
            .WithParticipant(identityAddress)
            .WithIdIn(ids);

        var relationships = await query.OrderAndPaginate(d => d.CreatedAt, paginationFilter, cancellationToken);

        return relationships;
    }

    public async Task Update(Relationship relationship)
    {
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(IEnumerable<Relationship> relationships)
    {
        _relationships.UpdateRange(relationships);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Add(Relationship relationship, CancellationToken cancellationToken)
    {
        await _relationships.AddAsync(relationship, cancellationToken);
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException != null && ex.InnerException.Message.Contains(ConstraintNames.ONLY_ONE_ACTIVE_RELATIONSHIP_BETWEEN_TWO_IDENTITIES))
                throw new DomainException(DomainErrors.RelationshipToTargetAlreadyExists());

            throw;
        }
    }

    public async Task<bool> RelationshipBetweenTwoIdentitiesExists(IdentityAddress identityAddressA,
        IdentityAddress identityAddressB, CancellationToken cancellationToken)
    {
        return await _readOnlyRelationships
            .BetweenParticipants(identityAddressA, identityAddressB)
            .Where(Relationship.CountsAsActive())
            .AnyAsync(cancellationToken);
    }

    public async Task DeleteRelationships(Expression<Func<Relationship, bool>> filter, CancellationToken cancellationToken)
    {
#pragma warning disable CS0618 // Type or member is obsolete; While it's true that there is an ExecuteDeleteAsync method in EF Core, it cannot be used here because it cannot be used in scenarios where table splitting is used. See https://github.com/dotnet/efcore/issues/28521 for the feature request that would allow this.
        await _relationships.Where(filter).BatchDeleteAsync(cancellationToken);
#pragma warning restore CS0618 // Type or member is obsolete
    }

    public async Task<IEnumerable<Relationship>> ListWithoutContent(Expression<Func<Relationship, bool>> filter, CancellationToken cancellationToken, bool track = false)
    {
        return await (track ? _relationships : _readOnlyRelationships)
            .Include(r => r.RelationshipTemplate)
            .Include(r => r.AuditLog)
            .AsSplitQuery()
            .Where(filter)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> ListWithoutContent<T>(Expression<Func<Relationship, bool>> filter, Expression<Func<Relationship, T>> selector, CancellationToken cancellationToken,
        bool track = false)
    {
        return await (track ? _relationships : _readOnlyRelationships)
            .Include(r => r.RelationshipTemplate)
            .Include(r => r.AuditLog)
            .AsSplitQuery()
            .Where(filter)
            .Select(selector)
            .ToListAsync(cancellationToken);
    }
}
