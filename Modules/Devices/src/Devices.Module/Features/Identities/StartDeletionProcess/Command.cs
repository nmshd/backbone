using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.StartDeletionProcess;

public class Command : IRequest<Response>
{
    public double? LengthOfGracePeriodInDays { get; init; }
}
