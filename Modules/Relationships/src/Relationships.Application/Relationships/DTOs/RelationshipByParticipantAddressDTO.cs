using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Entities;

namespace Backbone.Modules.Relationships.Application.Relationships.DTOs;
public class RelationshipByParticipantAddressDTO
{
    public string Peer { get; set; }
    public string RequestedBy { get; set; }
    public string TemplateId { get; set; }
    public RelationshipStatus Status { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? AnsweredAt { get; set; }
    public string CreatedByDevice { get; set; }
    public string AnsweredByDevice { get; set; }

    public RelationshipByParticipantAddressDTO(IdentityAddress participantAddress, Relationship relationship)
    {
        Peer = relationship.To == participantAddress ? relationship.From : relationship.To;
        RequestedBy = relationship.To == participantAddress ? "Peer" : "Self";
        TemplateId = relationship.RelationshipTemplateId;
        Status = relationship.Status;
        CreationDate = relationship.CreatedAt;
        AnsweredAt = relationship.Changes.GetLatestOfType(RelationshipChangeType.Creation).Response?.CreatedAt;
        CreatedByDevice = relationship.Changes.GetLatestOfType(RelationshipChangeType.Creation).Request.CreatedByDevice;
        AnsweredByDevice = relationship.Changes.GetLatestOfType(RelationshipChangeType.Creation).Response?.CreatedByDevice;
    }
}
