using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.DeleteTokensOfIdentity;
public class DeleteTokensOfIdentityCommand : IRequest
{
    public DeleteTokensOfIdentityCommand(IdentityAddress identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public IdentityAddress IdentityAddress { get; set; }
}
