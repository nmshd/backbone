namespace Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

public enum RelationshipStatus
{
    Pending = 10,
    Active = 20,
    Rejected = 30,
    Revoked = 40,
    Terminating = 50,
    Terminated = 60
}
