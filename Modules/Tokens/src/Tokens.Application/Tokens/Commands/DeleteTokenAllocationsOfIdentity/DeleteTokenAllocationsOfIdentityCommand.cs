using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.DeleteTokenAllocationsOfIdentity;

public class DeleteTokenAllocationsOfIdentityCommand : IRequest
{
    public required string IdentityAddress { get; init; }
}
