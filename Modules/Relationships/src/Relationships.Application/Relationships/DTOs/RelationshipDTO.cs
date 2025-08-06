using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Application.Relationships.DTOs;

public class RelationshipDTO : RelationshipMetadataDTO
{
    public RelationshipDTO(Relationship relationship) : base(relationship)
    {
        CreationContent = relationship.Details.CreationContent;
        CreationResponseContent = relationship.Details.CreationResponseContent;
    }

    public byte[]? CreationContent { get; set; }
    public byte[]? CreationResponseContent { get; set; }
}
