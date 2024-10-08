using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;

namespace Backbone.Modules.Relationships.Application;

public static class ApplicationErrors
{
    public static class Relationship
    {
        public static ApplicationError PeerIsToBeDeleted(string peerToBeDeleted)
        {
            return new ApplicationError("error.platform.validation.relationship.peerIsToBeDeleted",
                $"Cannot establish relationship with '{peerToBeDeleted}' because they are in status 'ToBeDeleted'.");
        }
    }
}
