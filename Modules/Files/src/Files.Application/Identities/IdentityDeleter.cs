using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Files.Application.Identities.Commands.DeleteFilesOfIdentity;
using MediatR;

namespace Backbone.Modules.Files.Application.Identities;

public class IdentityDeleter : IIdentityDeleter
{
    private readonly IMediator _mediator;

    public IdentityDeleter(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Delete(IdentityAddress identityAddress, IDeletionProcessLogger deletionProcessLogger)
    {
        await _mediator.Send(new DeleteFilesOfIdentityCommand(identityAddress));
        await deletionProcessLogger.LogDeletion(identityAddress, AggregateType.Files);
    }
}
