using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.StartDeletionProcessAsSupport;

public class StartDeletionProcessAsSupportCommand : IRequest<StartDeletionProcessAsSupportResponse>
{
    public StartDeletionProcessAsSupportCommand(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; set; }
}
