namespace Backbone.Modules.Relationships.Application;

public static class ApplicationErrors
{
    public static class Relationship
    {
        public static ApplicationError RelationshipToTargetAlreadyExists(string targetIdentity = "")
        {
            var targetIdentityString = string.IsNullOrEmpty(targetIdentity) ? "the target identity" : targetIdentity;

            return new ApplicationError("error.platform.validation.relationshipRequest.relationshipToTargetAlreadyExists", $"A relationship to {targetIdentityString} already exists.");
        }

        public static ApplicationError CannotSendRelationshipRequestToYourself()
        {
            return new ApplicationError("error.platform.validation.relationshipRequest.cannotSendRelationshipRequestToYourself", "The template you provided is your own. You cannot send a relationship request to yourself.");
        }

        public static ApplicationError CannotCreateRelationshipWhileTerminatedRelationshipExists(string terminatedRelationship = "")
        {
            var terminatedRelationshipString = string.IsNullOrEmpty(terminatedRelationship) ? "a terminated relationship" : terminatedRelationship;

            return new ApplicationError(
                "error.platform.validation.relationshipRequest.cannotCreateRelationshipWhileTerminatedRelationshipExists",
                $"Cannot create relationship while terminated relationship {terminatedRelationshipString} exists.");
        }
    }
}
