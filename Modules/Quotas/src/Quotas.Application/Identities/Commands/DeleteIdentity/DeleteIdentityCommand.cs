using MediatR;

namespace Backbone.Modules.Quotas.Application.Identities.Commands.DeleteIdentity;

public class DeleteIdentityCommand : IRequest
{
    public required string IdentityAddress { get; init; }
}
