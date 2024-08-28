using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.ListIdentities;

public class ListIdentitiesQuery : IRequest<ListIdentitiesResponse>
{
    public ListIdentitiesQuery(List<string>? addresses = null, IdentityStatus? status = null)
    {
        Addresses = addresses;
        Status = status;
    }

    public List<string>? Addresses { get; set; }
    public IdentityStatus? Status { get; set; }
}
