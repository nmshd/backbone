using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.IsIdentityOfUserDeleted;

public class IsIdentityOfUserDeletedQuery : IRequest<IsIdentityOfUserDeletedResponse>
{
    public IsIdentityOfUserDeletedQuery(string username)
    {
        Username = username;
    }

    public string Username { get; }
}
