using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.AnonymizeTokenAllocationsOfIdentity;

public class AnonymizeTokenAllocationsOfIdentityCommand : IRequest
{
    public AnonymizeTokenAllocationsOfIdentityCommand(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; set; }
}
