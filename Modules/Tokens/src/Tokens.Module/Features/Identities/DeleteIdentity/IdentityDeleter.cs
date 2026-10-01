using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Tokens.Module.Features.Tokens.AnonymizeTokenAllocationsOfIdentity;
using Backbone.Modules.Tokens.Module.Features.Tokens.AnonymizeTokensForIdentity;
using Backbone.Modules.Tokens.Module.Features.Tokens.DeleteTokensOfIdentity;
using MediatR;

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
        await _mediator.Send(new DeleteTokensOfIdentityCommand { IdentityAddress = identityAddress });
        await _mediator.Send(new AnonymizeTokensForIdentityCommand { IdentityAddress = identityAddress });
        await _mediator.Send(new AnonymizeTokenAllocationsOfIdentityCommand { IdentityAddress = identityAddress });
        await _deletionProcessLogger.LogDeletion(identityAddress, "Tokens");
    }
}
