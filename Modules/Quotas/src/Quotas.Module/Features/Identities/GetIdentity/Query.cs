using MediatR;

namespace Backbone.Modules.Quotas.Module.Features.Identities.GetIdentity;

public class GetIdentityQuery : IRequest<GetIdentityResponse>
{
    public required string Address { get; init; }
}
