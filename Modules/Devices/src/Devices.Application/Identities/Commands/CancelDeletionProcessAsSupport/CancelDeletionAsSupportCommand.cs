using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcessAsSupport;

public class CancelDeletionAsSupportCommand : IRequest<CancelDeletionAsSupportResponse>
{
    public CancelDeletionAsSupportCommand(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; set; }
}
