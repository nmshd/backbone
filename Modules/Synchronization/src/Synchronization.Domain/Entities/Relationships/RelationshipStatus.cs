namespace Backbone.Modules.Synchronization.Domain.Entities.Relationships;

public enum RelationshipStatus
{
    Pending = 10,
    Active = 20,
    Rejected = 30,
    Revoked = 40,
    Terminated = 50,
    DeletionProposed = 60,
    ReadyForDeletion = 70
}
