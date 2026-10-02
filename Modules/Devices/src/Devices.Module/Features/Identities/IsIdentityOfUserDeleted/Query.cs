using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.IsIdentityOfUserDeleted;

public class IsIdentityOfUserDeletedQuery : IRequest<IsIdentityOfUserDeletedResponse>
{
    public required string Username { get; init; }
}
