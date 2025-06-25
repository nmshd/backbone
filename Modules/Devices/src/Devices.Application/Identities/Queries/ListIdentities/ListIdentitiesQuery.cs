using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.ListIdentities;

public class ListIdentitiesQuery : IRequest<ListIdentitiesResponse>
{
    public required List<string>? Addresses { get; init; }
    public required IdentityStatus? Status { get; init; }
}
