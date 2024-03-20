using System.Linq.Expressions;
using Backbone.BuildingBlocks.Domain;
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

    public Relationship(RelationshipTemplate relationshipTemplate, IdentityAddress activeIdentity, DeviceId activeDevice, byte[]? creationContent)
    {
        if (activeIdentity == relationshipTemplate.CreatedBy)
            throw new DomainException(DomainErrors.CannotSendRelationshipRequestToYourself());

        Id = RelationshipId.New();
        RelationshipTemplateId = relationshipTemplate.Id;
        RelationshipTemplate = relationshipTemplate;

        From = activeIdentity;
        To = relationshipTemplate.CreatedBy;
        Status = RelationshipStatus.Pending;

        CreatedAt = SystemTime.UtcNow;

        CreationContent = creationContent;

        AuditLog = new List<RelationshipAuditLogEntry>
        {
            new(RelationshipAuditLogEntryReason.Creation, null, RelationshipStatus.Pending, activeIdentity, activeDevice)
        };
    }

    public RelationshipId Id { get; }
    public RelationshipTemplateId RelationshipTemplateId { get; }
    public RelationshipTemplate RelationshipTemplate { get; }

    public IdentityAddress From { get; }
    public IdentityAddress To { get; }

    public DateTime CreatedAt { get; }

    public RelationshipStatus Status { get; private set; }
    public byte[]? CreationContent { get; set; }
    public List<RelationshipAuditLogEntry> AuditLog { get; set; }

    #region Selectors

    public static Expression<Func<Relationship, bool>> HasParticipant(string identity)
    {
        return r => r.From == identity || r.To == identity;
    }

    #endregion

    public void Accept(IdentityAddress activeIdentity, DeviceId activeDevice)
    {
        if (Status != RelationshipStatus.Pending)
            throw new DomainException(DomainErrors.RelationshipIsNotInCorrectStatus(RelationshipStatus.Pending));

        if (To != activeIdentity)
            throw new DomainException(DomainErrors.CannotSendRelationshipRequestToYourself());

        Status = RelationshipStatus.Active;

        AuditLog.Add(new RelationshipAuditLogEntry(RelationshipAuditLogEntryReason.AcceptanceOfCreation, RelationshipStatus.Pending, RelationshipStatus.Active, activeIdentity, activeDevice));
    }
}
