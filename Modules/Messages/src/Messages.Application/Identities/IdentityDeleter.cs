using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Application.Messages.Commands.AnonymizeMessagesOfIdentity;
using MediatR;

namespace Backbone.Modules.Messages.Application.Identities;

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
        await _mediator.Send(new AnonymizeMessagesOfIdentityCommand(identityAddress));
        await _deletionProcessLogger.LogDeletion(identityAddress, "Messages");
    }
}
