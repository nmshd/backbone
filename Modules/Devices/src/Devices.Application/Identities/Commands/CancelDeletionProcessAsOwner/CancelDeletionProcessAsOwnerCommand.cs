using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcessAsOwner;

public class CancelDeletionProcessAsOwnerCommand : IRequest<CancelDeletionProcessAsOwnerResponse>
{
    public required string DeletionProcessId { get; init; }
}
