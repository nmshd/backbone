using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;

namespace Backbone.Modules.Relationships.Application;

public static class ApplicationErrors
{
    public static class Relationship
    {
        public static ApplicationError PeerIsToBeDeleted()
        {
            return new ApplicationError("error.platform.validation.relationshipRequest.peerIsToBeDeleted",
                "Cannot establish relationship with the owner of the template because they are in status 'ToBeDeleted'.");
        }

        public static ApplicationError RelationshipToTargetAlreadyExists(string targetIdentity = "")
        {
            var targetIdentityString = string.IsNullOrEmpty(targetIdentity) ? "the target identity" : targetIdentity;

            return new ApplicationError("error.platform.validation.relationshipRequest.relationshipToTargetAlreadyExists", $"A relationship to {targetIdentityString} already exists.");
        }

        public static ApplicationError CannotSendRelationshipRequestToYourself()
        {
            return new ApplicationError("error.platform.validation.relationshipRequest.cannotSendRelationshipRequestToYourself", "The template you provided is your own. You cannot send a relationship request to yourself.");
        }
    }
}
