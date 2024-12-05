using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.AnonymizeTokensForIdentity;

public class AnonymizeTokensForIdentityCommand : IRequest
{
    public AnonymizeTokensForIdentityCommand(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; set; }
}
