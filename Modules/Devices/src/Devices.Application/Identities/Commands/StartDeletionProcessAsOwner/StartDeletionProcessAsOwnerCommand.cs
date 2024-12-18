using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.StartDeletionProcessAsOwner;

public class StartDeletionProcessAsOwnerCommand : IRequest<StartDeletionProcessAsOwnerResponse>
{
    public double? LengthOfGracePeriodInDays { get; set; }
}
