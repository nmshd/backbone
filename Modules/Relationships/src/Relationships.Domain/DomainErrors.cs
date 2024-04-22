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
        return new DomainError("error.platform.validation.relationshipRequest.cannotSendRelationshipRequestToYourself",
            "You cannot send a relationship request to yourself.");
    }

    public static DomainError CannotAcceptOrRejectRelationshipRequestAddressedToSomeoneElse()
    {
        return new DomainError("error.platform.validation.relationshipRequest.cannotAcceptOrRejectRelationshipRequestAddressedToSomeoneElse",
            "You cannot accept or reject a relationship request that is addressed to someone else.");
    }

    public static DomainError CannotRevokeRelationshipRequestNotCreatedByYourself()
    {
        return new DomainError("error.platform.validation.relationshipRequest.cannotRevokeRelationshipRequestNotCreatedByYourself",
            "You cannot revoke a relationship request that was not created by yourself.");
    }

    public static DomainError RelationshipIsNotInCorrectStatus(RelationshipStatus expectedStatus)
    {
        return new DomainError("error.platform.validation.relationshipRequest.relationshipIsNotInCorrectStatus",
            $"The relationship has to be in status '{expectedStatus}' to perform this action.");
    }

    public static DomainError RelationshipToTargetAlreadyExists(string targetIdentity)
    {
        return new DomainError("error.platform.validation.relationshipRequest.relationshipToTargetAlreadyExists",
            $"A relationship to '{targetIdentity}' already exists. If the relationship is terminated, you can reactivate it.");
    }

    public static DomainError CannotReactivateAnAlreadyRequestedReactivation()
    {
        return new DomainError("error.platform.validation.relationshipRequest.cannotReactivateAnAlreadyRequestedReactivation",
            $"You cannot reactivate an already requested reactivation.");
    }
}
