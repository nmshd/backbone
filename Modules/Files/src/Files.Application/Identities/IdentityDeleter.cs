using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Files.Application.Identities.Commands.AnonymizeCreatedByOfFiles;
using Backbone.Modules.Files.Application.Identities.Commands.DeleteFilesOfIdentity;
using MediatR;

namespace Backbone.Modules.Files.Application.Identities;

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
        await _mediator.Send(new DeleteFilesOfIdentityCommand { IdentityAddress = identityAddress });
        await _mediator.Send(new AnonymizeCreatedByOfFilesCommand { IdentityAddress = identityAddress });
        await _deletionProcessLogger.LogDeletion(identityAddress, "Files");
    }
}
