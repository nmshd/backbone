namespace Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

public enum RelationshipAuditLogEntryReason
{
    Creation = 0,
    AcceptanceOfCreation = 1,
    RejectionOfCreation = 2,
    RevocationOfCreation = 3,
    Termination = 4,
    Decomposed = 5
}
