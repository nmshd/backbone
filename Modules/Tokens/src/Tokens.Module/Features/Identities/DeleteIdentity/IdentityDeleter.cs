using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;
using AnonymizeTokenAllocationsOfIdentity = Backbone.Modules.Tokens.Module.Features.Tokens.AnonymizeTokenAllocationsOfIdentity;
using AnonymizeTokensForIdentity = Backbone.Modules.Tokens.Module.Features.Tokens.AnonymizeTokensForIdentity;
using DeleteTokensOfIdentity = Backbone.Modules.Tokens.Module.Features.Tokens.DeleteTokensOfIdentity;

namespace Backbone.Modules.Tokens.Module.Features.Identities.DeleteIdentity;

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
        await _mediator.Send(new DeleteTokensOfIdentity.Command { IdentityAddress = identityAddress });
        await _mediator.Send(new AnonymizeTokensForIdentity.Command { IdentityAddress = identityAddress });
        await _mediator.Send(new AnonymizeTokenAllocationsOfIdentity.Command { IdentityAddress = identityAddress });
        await _deletionProcessLogger.LogDeletion(identityAddress, "Tokens");
    }
}
