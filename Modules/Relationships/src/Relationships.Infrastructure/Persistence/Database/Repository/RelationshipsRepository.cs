using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Application.Infrastructure;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Backbone.Modules.Relationships.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Relationships.Infrastructure.Persistence.Database.Repository;

public class RelationshipsRepository : IRelationshipsRepository
{
    private readonly DbSet<Relationship> _relationships;
    private readonly IQueryable<Relationship> _readOnlyRelationships;
    private readonly RelationshipsDbContext _dbContext;
    private readonly DbSet<RelationshipTemplateAllocation> _relationshipTemplateAllocations;

    public RelationshipsRepository(RelationshipsDbContext dbContext)
    {
        _relationships = dbContext.Relationships;
        _readOnlyRelationships = dbContext.Relationships.AsNoTracking();
        _relationshipTemplateAllocations = dbContext.RelationshipTemplateAllocations;
        _dbContext = dbContext;
    }

    public async Task<Relationship> FindRelationship(RelationshipId id, IdentityAddress identityAddress,
        CancellationToken cancellationToken, bool track = false)
    {
        var relationship = await (track ? _relationships : _readOnlyRelationships)
            .IncludeAll(_dbContext)
            .WithParticipant(identityAddress)
            .FirstWithId(id, cancellationToken);

        return relationship;
    }

    public async Task<DbPaginationResult<Relationship>> FindRelationshipsWithIds(IEnumerable<RelationshipId> ids,
        IdentityAddress identityAddress, PaginationFilter paginationFilter, CancellationToken cancellationToken, bool track = false)
    {
        var query = (track ? _relationships : _readOnlyRelationships)
            .AsQueryable()
            .IncludeAll(_dbContext)
            .WithParticipant(identityAddress)
            .WithIdIn(ids);

        var relationships = await query.OrderAndPaginate(d => d.CreatedAt, paginationFilter, cancellationToken);

        return relationships;
    }

    public async Task Update(Relationship relationship)
    {
        _relationships.Update(relationship);
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
                throw new DomainException(DomainErrors.RelationshipToTargetAlreadyExists(relationship.To));

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
        await _relationships.Where(filter).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<IEnumerable<Relationship>> FindRelationships(Expression<Func<Relationship, bool>> filter, CancellationToken cancellationToken)
    {
        return await _relationships.Where(filter).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RelationshipTemplateAllocation>> FindRelationshipTemplateAllocations(Expression<Func<RelationshipTemplateAllocation, bool>> filter,
        CancellationToken cancellationToken)
    {
        return await _relationshipTemplateAllocations.Where(filter).ToListAsync(cancellationToken);
    }

    public async Task UpdateRelationshipTemplateAllocations(List<RelationshipTemplateAllocation> templateAllocations, CancellationToken cancellationToken)
    {
        _relationshipTemplateAllocations.UpdateRange(templateAllocations);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
