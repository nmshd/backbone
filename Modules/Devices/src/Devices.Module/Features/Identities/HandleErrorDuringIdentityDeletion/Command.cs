using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.HandleErrorDuringIdentityDeletion;

public class Command : IRequest
{
    public required string IdentityAddress { get; init; }
    public required string ErrorMessage { get; init; }
}
