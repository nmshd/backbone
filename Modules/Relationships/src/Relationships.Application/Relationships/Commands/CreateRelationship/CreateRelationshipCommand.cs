using Backbone.BuildingBlocks.Application.Attributes;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.CreateRelationship;

[ApplyQuotasForMetrics("NumberOfRelationships")]
public class CreateRelationshipCommand : IRequest<CreateRelationshipResponse>
{
    public required string RelationshipTemplateId { get; set; }
    public byte[]? Content { get; set; }
}
