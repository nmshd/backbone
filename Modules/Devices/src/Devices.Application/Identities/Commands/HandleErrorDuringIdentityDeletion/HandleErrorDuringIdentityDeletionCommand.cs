using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.HandleErrorDuringIdentityDeletion;

public class HandleErrorDuringIdentityDeletionCommand : IRequest
{
    public required string IdentityAddress { get; set; }
    public required string ErrorMessage { get; set; }
}
