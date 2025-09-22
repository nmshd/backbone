using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.StartDeletionProcess;

public class StartDeletionProcessCommand : IRequest<StartDeletionProcessResponse>
{
    public double? LengthOfGracePeriodInDays { get; init; }
}
