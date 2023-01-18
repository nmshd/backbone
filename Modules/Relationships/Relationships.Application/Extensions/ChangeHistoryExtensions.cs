using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Relationships.Domain.Entities;
using ApplicationException = Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Relationships.Application.Extensions;

public static class ChangeHistoryExtensions
{
    public static RelationshipChange GetLatestOpenOfTypeOrNull(this IRelationshipChangeLog changes, RelationshipChangeType type)
    {
        var change = changes.LastOrDefault(c => c.Type == type && c.Status == RelationshipChangeStatus.Pending);
        return change;
    }


    public static RelationshipChange GetLatestOfType(this IRelationshipChangeLog changes, RelationshipChangeType type)
    {
        var change = changes.LastOrDefault(c => c.Type == type);

        if (change == null)
            throw new ApplicationException(GenericApplicationErrors.NotFound(nameof(RelationshipChange)));

        return change;
    }
}
