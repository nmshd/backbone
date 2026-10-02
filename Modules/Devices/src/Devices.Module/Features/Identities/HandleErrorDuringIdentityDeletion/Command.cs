using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.HandleErrorDuringIdentityDeletion;

public class HandleErrorDuringIdentityDeletionCommand : IRequest
{
    public required string IdentityAddress { get; init; }
    public required string ErrorMessage { get; init; }
}
