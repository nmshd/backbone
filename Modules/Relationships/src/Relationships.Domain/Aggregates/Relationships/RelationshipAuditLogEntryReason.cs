namespace Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

public enum RelationshipAuditLogEntryReason
{
    Creation = 0,
    AcceptanceOfCreation = 1,
    RejectionOfCreation = 2,
    RevocationOfCreation = 3,
    Termination = 4,
    Reactivation = 5,
    AcceptanceOfReactivation = 6,
    RejectionOfReactivation = 7,
    RevocationOfReactivation = 8,
    Decomposition = 9
}
