using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.ListIdentities;

public class ListIdentitiesQuery : IRequest<ListIdentitiesResponse>
{
    public ListIdentitiesQuery(IEnumerable<IdentityAddress> addresses)
    {
        Addresses = addresses;
    }

    public IEnumerable<IdentityAddress> Addresses { get; set; }
}
