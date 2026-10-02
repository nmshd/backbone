using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.GetIdentity;

public class GetIdentityQuery : IRequest<GetIdentityResponse>
{
    public required string Address { get; init; }
}
