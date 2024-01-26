using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Relationships.Domain.Entities;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Relationships.Application.Extensions;

public static class ChangeHistoryExtensions
{
    public static RelationshipChange? GetLatestOpenOfTypeOrNull(this IRelationshipChangeLog changes, RelationshipChangeType type)
    {
        var change = changes.LastOrDefault(c => c.Type == type && c.Status == RelationshipChangeStatus.Pending);
        return change;
    }


    public static RelationshipChange GetLatestOfType(this IRelationshipChangeLog changes, RelationshipChangeType type)
    {
        var change = changes.LastOrDefault(c => c.Type == type) ?? throw new ApplicationException(GenericApplicationErrors.NotFound(nameof(RelationshipChange)));
        return change;
    }
}
