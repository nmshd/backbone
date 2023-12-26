using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.DeleteTokensOfIdentity;
public class DeleteTokensOfIdentityCommand(IdentityAddress identityAddress) : IRequest
{
    public IdentityAddress IdentityAddress { get; set; } = identityAddress;
}
