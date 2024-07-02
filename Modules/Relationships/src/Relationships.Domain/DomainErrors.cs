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

    public static DomainError RelationshipIsNotInCorrectStatus(RelationshipStatus[] expectedStatuses)
    {
        return new DomainError("error.platform.validation.relationshipRequest.relationshipIsNotInCorrectStatus",
            $"The relationship has to be in status '{string.Join(" or ", expectedStatuses)}' to perform this action.");
    }

    public static DomainError RelationshipToTargetAlreadyExists(string targetIdentity)
    {
        return new DomainError("error.platform.validation.relationshipRequest.relationshipToTargetAlreadyExists",
            $"A relationship to '{targetIdentity}' already exists. If the relationship is terminated, you can reactivate it.");
    }

    public static DomainError NoRevocableReactivationRequestExists(string activeIdentity)
    {
        return new DomainError("error.platform.validation.relationshipRequest.noRevocableReactivationRequestExists",
            $"There is no pending reactivation request or you are not allowed to revoke it. A reactivation request can only be revoked by the identity that request it.");
    }

    public static DomainError CannotRequestReactivationWhenThereIsAnOpenReactivationRequest()
    {
        return new DomainError("error.platform.validation.relationshipRequest.cannotRequestReactivationWhenThereIsAnOpenReactivationRequest",
            $"You cannot request reactivation when there is an open reactivation request.");
    }

    public static DomainError NoAcceptableRelationshipReactivationRequestExists()
    {
        return new DomainError("error.platform.validation.relationshipRequest.noAcceptableRelationshipReactivationRequestExists",
            "There is no pending reactivation request or you are not allowed to accept it. A reactivation request can only be accepted by the identity that did not request it.");
    }

    public static DomainError NoRejectableReactivationRequestExists()
    {
        return new DomainError("error.platform.validation.relationshipRequest.noRejectableReactivationRequestExists",
            "There is no pending reactivation request or you are not allowed to revoke it. A reactivation request can only be rejected by the identity that did not request it.");
    }

    public static DomainError RequestingIdentityDoesNotBelongToRelationship()
    {
        return new DomainError("error.platform.validation.relationshipRequest.requestingIdentityDoesNotBelongToRelationship",
            $"Requesting identity does not belong to the relationship.");
    }

    public static DomainError RelationshipAlreadyDecomposed()
    {
        return new DomainError("error.platform.validation.relationshipRequest.relationshipAlreadyDecomposed",
            $"You already decomposed this Relationship.");
    }
}
