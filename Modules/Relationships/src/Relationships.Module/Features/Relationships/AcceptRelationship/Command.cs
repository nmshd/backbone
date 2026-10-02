using Backbone.BuildingBlocks.Application.Attributes;
using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.AcceptRelationship;

[ApplyQuotasForMetrics("NumberOfRelationships")]
public class AcceptRelationshipCommand : IRequest<AcceptRelationshipResponse>
{
    public required string RelationshipId { get; init; }
    public required byte[]? CreationResponseContent { get; init; }
}
