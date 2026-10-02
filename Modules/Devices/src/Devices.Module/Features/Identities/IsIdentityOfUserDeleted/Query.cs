using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.IsIdentityOfUserDeleted;

public class Query : IRequest<Response>
{
    public required string Username { get; init; }
}
