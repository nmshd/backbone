using Backbone.BuildingBlocks.Application.Attributes;
using Backbone.Relationships.Domain.Ids;
using MediatR;

namespace Backbone.Relationships.Application.Relationships.Commands.CreateRelationship;

[ApplyQuotasForMetrics("NumberOfRelationships")]
public class CreateRelationshipCommand : IRequest<CreateRelationshipResponse>
{
    public RelationshipTemplateId RelationshipTemplateId { get; set; }
    public byte[] Content { get; set; }
}
