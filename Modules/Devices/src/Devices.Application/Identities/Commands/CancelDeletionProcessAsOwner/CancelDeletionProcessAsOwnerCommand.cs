using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcessAsOwner;

public class CancelDeletionProcessAsOwnerCommand : IRequest<CancelDeletionProcessAsOwnerResponse>
{
    public CancelDeletionProcessAsOwnerCommand(string address, string deletionProcessId)
    {
        Address = address;
        DeletionProcessId = deletionProcessId;
    }

    public string Address { get; set; }
    public string DeletionProcessId { get; set; }
}
