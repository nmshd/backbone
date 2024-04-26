using Backbone.BuildingBlocks.Application.Attributes;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.AcceptRelationship;

[ApplyQuotasForMetrics("NumberOfRelationships")]
public class AcceptRelationshipCommand : IRequest<AcceptRelationshipResponse>
{
    public required string RelationshipId { get; set; }
    public byte[]? CreationResponseContent { get; set; }
}
