using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;

namespace Backbone.Modules.Relationships.Application.Relationships.DTOs;

public class RelationshipDTO : IMapTo<Relationship>
{
    public RelationshipDTO(Relationship relationship)
    {
        Id = relationship.Id;
        RelationshipTemplateId = relationship.RelationshipTemplateId;
        From = relationship.From;
        To = relationship.To;
        CreatedAt = relationship.CreatedAt;
        Status = relationship.Status;
        CreationContent = relationship.CreationContent;
        CreationResponseContent = relationship.CreationResponseContent;
        AuditLog = relationship.AuditLog.Select(a => new RelationshipAuditLogEntryDTO(a)).ToList();
    }

    public RelationshipId Id { get; set; }
    public RelationshipTemplateId RelationshipTemplateId { get; set; }

    public IdentityAddress From { get; set; }
    public IdentityAddress To { get; set; }

    public byte[]? CreationContent { get; set; }
    public byte[]? CreationResponseContent { get; set; }

    public DateTime CreatedAt { get; set; }

    public RelationshipStatus Status { get; set; }

    public List<RelationshipAuditLogEntryDTO> AuditLog { get; set; }
}
