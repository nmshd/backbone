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
    }
}
