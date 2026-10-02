using MediatR;

namespace Backbone.Modules.Quotas.Module.Features.Identities.GetIdentity;

public class Query : IRequest<Response>
{
    public required string Address { get; init; }
}
