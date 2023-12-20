using System.Linq.Expressions;
using Backbone.Modules.Relationships.Domain.Entities;

namespace Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
public interface IRelationshipTemplateAllocationsRepository
{
    public Task DeleteAllocations(Expression<Func<RelationshipTemplateAllocation, bool>> filter, CancellationToken cancellationToken);    
}
