using System.Linq.Expressions;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Relationships.Infrastructure.Persistence.Database.Repository;
public class RelationshipTemplateAllocationsRepository : IRelationshipTemplateAllocationsRepository
{
    private readonly DbSet<RelationshipTemplateAllocation> _allocations;

    public RelationshipTemplateAllocationsRepository(RelationshipsDbContext dbContext)
    {
        _allocations = dbContext.RelationshipTemplateAllocations;
    }

    public async Task DeleteAllocations(Expression<Func<RelationshipTemplateAllocation, bool>> filter, CancellationToken cancellationToken)
    {
        await _allocations.Where(filter).ExecuteDeleteAsync(cancellationToken);
    }
}
