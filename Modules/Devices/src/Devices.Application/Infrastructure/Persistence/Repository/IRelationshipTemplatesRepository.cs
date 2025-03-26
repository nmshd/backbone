using System.Linq.Expressions;
using Backbone.Modules.Devices.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;

public interface IRelationshipTemplatesRepository
{
    Task<bool> AllocationExists(Expression<Func<RelationshipTemplateAllocation, bool>> filter, CancellationToken cancellationToken);
}
