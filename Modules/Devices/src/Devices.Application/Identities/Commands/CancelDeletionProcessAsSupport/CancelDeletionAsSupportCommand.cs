using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcessAsSupport;

public class CancelDeletionAsSupportCommand : IRequest<CancelDeletionAsSupportResponse>
{
    public required string Address { get; init; }
    public required string DeletionProcessId { get; init; }
}
