using MediatR;

namespace Backbone.Modules.Quotas.Application.Identities.Queries.GetIdentity;

public class GetIdentityQuery : IRequest<GetIdentityResponse>
{
    public required string Address { get; init; }
}
