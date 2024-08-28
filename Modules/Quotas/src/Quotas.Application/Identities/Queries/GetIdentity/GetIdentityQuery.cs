using MediatR;

namespace Backbone.Modules.Quotas.Application.Identities.Queries.GetIdentity;

public class GetIdentityQuery : IRequest<GetIdentityResponse>
{
    public GetIdentityQuery(string address)
    {
        Address = address;
    }

    public string Address { get; set; }
}
