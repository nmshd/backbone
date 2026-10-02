using Backbone.BuildingBlocks.Application.Attributes;
using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.CreateRelationship;

[ApplyQuotasForMetrics("NumberOfRelationships")]
public class CreateRelationshipCommand : IRequest<CreateRelationshipResponse>
{
    public required string RelationshipTemplateId { get; init; }
    public byte[]? CreationContent { get; init; }
}
