using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;

namespace Backbone.Modules.Relationships.Application;

public static class ApplicationErrors
{
    public static class Relationship
    {
        public static ApplicationError PeerIsToBeDeleted()
        {
            return new ApplicationError("error.platform.validation.relationship.peerIsToBeDeleted",
                "Cannot establish relationship with the owner of the template because the owner is in status 'ToBeDeleted'.");
        }
    }
}
