using Backbone.Modules.Relationships.Domain.Entities;

namespace Backbone.Modules.Relationships.Application.Tests.Extensions;

public static class IChangeLogExtensions
{
    public static RelationshipChange? GetOpenCreation(this IRelationshipChangeLog changes)
    {
        return changes.GetLatestOfTypeOrNull(RelationshipChangeType.Creation, c => !c.IsCompleted);
    }
}
