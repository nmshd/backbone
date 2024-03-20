using Backbone.BuildingBlocks.Application.Attributes;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.CreateRelationship;

[ApplyQuotasForMetrics("NumberOfRelationships")]
public class CreateRelationshipCommand : IRequest<CreateRelationshipResponse>
{
    public required RelationshipTemplateId RelationshipTemplateId { get; set; }
    public byte[]? Content { get; set; }
}
