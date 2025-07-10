using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.StartDeletionProcessAsSupport;

public class StartDeletionProcessAsSupportCommand : IRequest<StartDeletionProcessAsSupportResponse>
{
    public required string IdentityAddress { get; init; }
}
