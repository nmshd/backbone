using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcessAsOwner;

public class CancelDeletionProcessAsOwnerCommand : IRequest<CancelDeletionProcessAsOwnerResponse>
{
    public CancelDeletionProcessAsOwnerCommand(string deletionProcessId)
    {
        DeletionProcessId = deletionProcessId;
    }

    public string DeletionProcessId { get; set; }
}
