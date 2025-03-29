using System.Linq.Expressions;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.Relationships;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Repository;

public class RelationshipTemplatesRepository : IRelationshipTemplatesRepository
{
    private readonly IQueryable<RelationshipTemplateAllocation> _readOnlyRelationshipTemplateAllocations;

    public RelationshipTemplatesRepository(DevicesDbContext dbContext)
    {
        _readOnlyRelationshipTemplateAllocations = dbContext.RelationshipTemplateAllocations.AsNoTracking();
    }

    public async Task<bool> AllocationExists(Expression<Func<RelationshipTemplateAllocation, bool>> filter, CancellationToken cancellationToken)
    {
        return await _readOnlyRelationshipTemplateAllocations.AnyAsync(filter, cancellationToken);
    }
}
