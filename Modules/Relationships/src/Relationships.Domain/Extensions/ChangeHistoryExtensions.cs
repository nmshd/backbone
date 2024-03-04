using Backbone.Modules.Relationships.Domain.Entities;

namespace Backbone.Modules.Relationships.Domain.Extensions;

public static class ChangeHistoryExtensions
{
    public static RelationshipChange? GetOpenTerminationOrNull(this IRelationshipChangeLog changes)
    {
        return changes.GetLatestOfTypeOrNull(RelationshipChangeType.Termination, c => !c.IsCompleted);
    }

    public static RelationshipChange? GetPendingChangeOrNull(this IRelationshipChangeLog changes)
    {
        return changes.GetLatestOfTypeOrNull(RelationshipChangeType.Termination, c => !c.IsCompleted);
    }
}
