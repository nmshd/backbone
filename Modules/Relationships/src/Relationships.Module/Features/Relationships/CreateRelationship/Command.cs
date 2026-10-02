using Backbone.BuildingBlocks.Application.Attributes;
using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.CreateRelationship;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("CreateRelationshipCommand")]
[ApplyQuotasForMetrics("NumberOfRelationships")]
public class Command : IRequest<Response>
{
    public required string RelationshipTemplateId { get; init; }
    public byte[]? CreationContent { get; init; }
}
