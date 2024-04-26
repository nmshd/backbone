using Backbone.BuildingBlocks.Application.Attributes;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.AcceptRelationshipReactivation;

[ApplyQuotasForMetrics("NumberOfRelationships")]
public class AcceptRelationshipReactivationCommand : IRequest<AcceptRelationshipReactivationResponse>
{
    public required string RelationshipId { get; set; }
}
