﻿using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.Modules.Relationships.Domain.Entities;
using Backbone.Modules.Relationships.Domain.Ids;

namespace Backbone.Modules.Relationships.Domain.Errors;

public static class DomainErrors
{
    public static DomainError ChangeRequestCannotBeAcceptedByCreator()
    {
        return new DomainError("error.platform.relationships.changeRequestCannotBeAcceptedByCreator", "A change request cannot be accepted by its creator.");
    }

    public static DomainError ChangeRequestCannotBeRejectedByCreator()
    {
        return new DomainError("error.platform.relationships.changeRequestCannotBeRejectedByCreator", "A change request cannot be rejected by its creator.");
    }

    public static DomainError ChangeRequestCanOnlyBeRevokedByCreator()
    {
        return new DomainError("error.platform.relationships.changeRequestCanOnlyBeRevokedByCreator", "A change request can only be revoked by its creator.");
    }

    public static DomainError ChangeRequestIsAlreadyCompleted(RelationshipChangeStatus? changeStatus = null)
    {
        return new DomainError("error.platform.relationships.changeRequestIsAlreadyCompleted",
            $"This change is already completed. (current status: '{changeStatus}').");
    }

    public static DomainError OnlyActiveRelationshipsCanBeTerminated()
    {
        return new DomainError("error.platform.relationships.onlyActiveRelationshipsCanBeTerminated", "Only active relationships can be terminated.");
    }

    public static DomainError PendingChangeAlreadyExists(RelationshipChangeId? changeId = null)
    {
        return new DomainError("error.platform.relationships.pendingChangeAlreadyExists", $"There is already a pending change for this relationship. Change ID: {changeId}");
    }

    public static DomainError ContentIsRequiredForCompletingRelationships()
    {
        return new DomainError("error.platform.relationships.contentIsRequiredForCompletingRelationships", "The content property is required for accepting a relationship.");
    }

    public static DomainError MaxNumberOfAllocationsExhausted()
    {
        return new DomainError("error.platform.validation.relationshipTemplate.maxNumberOfAllocationsExhausted", "The maximum number of allocations (maxNumberOfAllocations) of the template you are trying to read is exhausted.");
    }
}
