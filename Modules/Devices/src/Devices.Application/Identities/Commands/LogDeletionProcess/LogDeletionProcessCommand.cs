using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.LogDeletionProcess;
public class LogDeletionProcessCommand : IRequest
{
    public LogDeletionProcessCommand(IdentityAddress identityAddress, AggregateType aggregateType)
    {
        IdentityAddress = identityAddress;
        AggregateType = aggregateType;
    }

    public string IdentityAddress { get; set; }
    public AggregateType AggregateType { get; set; }
}
