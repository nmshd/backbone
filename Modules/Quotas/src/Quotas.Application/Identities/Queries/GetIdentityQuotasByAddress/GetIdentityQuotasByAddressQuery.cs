using MediatR;

namespace Backbone.Modules.Quotas.Application.Identities.Queries.GetIdentityQuotasByAddress;
public class GetIdentityQuotasByAddressQuery : IRequest<GetIdentityQuotasByAddressResponse>
{
    public GetIdentityQuotasByAddressQuery(string address)
    {
        Address = address;
    }
    public string Address { get; set; }
}
