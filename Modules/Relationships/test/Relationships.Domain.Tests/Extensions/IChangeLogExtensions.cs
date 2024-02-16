﻿using Backbone.Modules.Relationships.Domain.Entities;

namespace Backbone.Modules.Relationships.Domain.Tests.Extensions;

public static class IChangeLogExtensions
{
    public static RelationshipChange? GetOpenCreation(this IRelationshipChangeLog changes)
    {
        return changes.GetLatestOfTypeOrNull(RelationshipChangeType.Creation, c => !c.IsCompleted);
    }

    public static RelationshipChange? GetOpenTermination(this IRelationshipChangeLog changes)
    {
        return changes.GetLatestOfTypeOrNull(RelationshipChangeType.Termination, c => !c.IsCompleted);
    }

    public static RelationshipChange? GetOpenTerminationCancellation(this IRelationshipChangeLog changes)
    {
        return changes.GetLatestOfTypeOrNull(RelationshipChangeType.TerminationCancellation, c => !c.IsCompleted);
    }
}
