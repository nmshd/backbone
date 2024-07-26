using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.LogDeletionProcess;

public class LogDeletionProcessCommand : IRequest
{
    public LogDeletionProcessCommand(IdentityAddress identityAddress, string aggregateType)
    {
        IdentityAddress = identityAddress;
        AggregateType = aggregateType;
    }

    public string IdentityAddress { get; set; }
    public string AggregateType { get; set; }
}
