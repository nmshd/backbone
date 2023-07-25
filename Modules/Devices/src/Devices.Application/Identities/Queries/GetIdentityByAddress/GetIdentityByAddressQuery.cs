using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetIdentityByAddress;
public class GetIdentityByAddressQuery : IRequest<GetIdentityByAddressResponse>
{
    public GetIdentityByAddressQuery(string address)
    {
        Address = address;
    }
    public string Address { get; set; }
}
