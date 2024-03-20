using System.Linq.Expressions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Backbone.Tooling;

namespace Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

public class Relationship
{
    // ReSharper disable once UnusedMember.Local
    private Relationship()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Id = null!;
        RelationshipTemplateId = null!;
        RelationshipTemplate = null!;
        From = null!;
        To = null!;
        AuditLog = null!;
    }

    public Relationship(RelationshipTemplate relationshipTemplate, IdentityAddress from, DeviceId fromDevice, byte[]? requestContent)
    {
        Id = RelationshipId.New();
        RelationshipTemplateId = relationshipTemplate.Id;
        RelationshipTemplate = relationshipTemplate;

        From = from;
        To = relationshipTemplate.CreatedBy;
        Status = RelationshipStatus.Pending;

        CreatedAt = SystemTime.UtcNow;

        AuditLog = new List<RelationshipAuditLogEntry>
        {
            new(RelationshipAuditLogEntryReason.Creation, null, RelationshipStatus.Pending, from, fromDevice)
        };
    }

    public RelationshipId Id { get; }
    public RelationshipTemplateId RelationshipTemplateId { get; }
    public RelationshipTemplate RelationshipTemplate { get; }

    public IdentityAddress From { get; }
    public IdentityAddress To { get; }

    public DateTime CreatedAt { get; }

    public RelationshipStatus Status { get; private set; }
    public List<RelationshipAuditLogEntry> AuditLog { get; set; }

    #region Selectors

    public static Expression<Func<Relationship, bool>> HasParticipant(string identity)
    {
        return r => r.From == identity || r.To == identity;
    }

    #endregion
}
