using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcessAsSupport;

public class CancelDeletionAsSupportCommand : IRequest<CancelDeletionAsSupportResponse>
{
    public CancelDeletionAsSupportCommand(string address, string deletionProcessId)
    {
        Address = address;
        DeletionProcessId = deletionProcessId;
    }

    public string Address { get; set; }
    public string DeletionProcessId { get; set; }
}
