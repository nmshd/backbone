using MediatR;

namespace Backbone.Modules.Quotas.Module.Features.Identities.DeleteIdentity;

public class Command : IRequest
{
    public required string IdentityAddress { get; init; }
}
