using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Domain;

public static class DomainErrors
{
    public static DomainError MaxNumberOfAllocationsExhausted()
    {
        return new DomainError("error.platform.validation.relationshipTemplate.maxNumberOfAllocationsExhausted",
            "The maximum number of allocations (maxNumberOfAllocations) of the template you are trying to read is exhausted.");
    }

    public static DomainError CannotSendRelationshipRequestToYourself()
    {
        return new DomainError("error.platform.validation.relationshipRequest.cannotAcceptOwnRelationshipRequest",
            "You cannot accept a relationship request that was created by yourself.");
    }

    public static DomainError RelationshipIsNotInCorrectStatus(RelationshipStatus expectedStatus)
    {
        return new DomainError("error.platform.validation.relationshipRequest.relationshipIsNotInCorrectStatus",
            $"The relationship has to be in status '{expectedStatus}' to perform this action.");
    }
}
