using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.StartDeletionProcess;

public class StartDeletionProcessCommand : IRequest<StartDeletionProcessResponse>
{
    public double? LengthOfGracePeriodInDays { get; init; }
}
