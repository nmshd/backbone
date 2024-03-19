using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcessAsSupport;

public class CancelDeletionAsSupportCommand : IRequest<CancelDeletionAsSupportResponse>
{
    public CancelDeletionAsSupportCommand(string deletionProcessId)
    {
        DeletionProcessId = deletionProcessId;
    }

    public string DeletionProcessId { get; set; }
}
