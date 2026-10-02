using Backbone.BuildingBlocks.Application.Attributes;
using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.AcceptRelationship;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("AcceptRelationshipCommand")]
[ApplyQuotasForMetrics("NumberOfRelationships")]
public class Command : IRequest<Response>
{
    public required string RelationshipId { get; init; }
    public required byte[]? CreationResponseContent { get; init; }
}
