using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.IsIdentityOfUserDeleted;

public class IsIdentityOfUserDeletedQuery : IRequest<IsIdentityOfUserDeletedResponse>
{
    public required string Username { get; init; }
}
