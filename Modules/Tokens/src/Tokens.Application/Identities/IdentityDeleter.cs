using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Tokens.Application.Tokens.Commands.AnonymizeTokensForIdentity;
using Backbone.Modules.Tokens.Application.Tokens.Commands.DeleteTokenAllocationsOfIdentity;
using Backbone.Modules.Tokens.Application.Tokens.Commands.DeleteTokensOfIdentity;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Identities;

public class IdentityDeleter : IIdentityDeleter
{
    private readonly IMediator _mediator;
    private readonly IDeletionProcessLogger _deletionProcessLogger;

    public IdentityDeleter(IMediator mediator, IDeletionProcessLogger deletionProcessLogger)
    {
        _mediator = mediator;
        _deletionProcessLogger = deletionProcessLogger;
    }

    public async Task Delete(IdentityAddress identityAddress)
    {
        await _mediator.Send(new DeleteTokensOfIdentityCommand { IdentityAddress = identityAddress });
        await _mediator.Send(new AnonymizeTokensForIdentityCommand { IdentityAddress = identityAddress });
        await _mediator.Send(new DeleteTokenAllocationsOfIdentityCommand { IdentityAddress = identityAddress });
        await _deletionProcessLogger.LogDeletion(identityAddress, "Tokens");
    }
}
