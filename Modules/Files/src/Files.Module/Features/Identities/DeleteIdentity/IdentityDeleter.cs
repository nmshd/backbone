using AnonymizeCreatedByOfFiles = Backbone.Modules.Files.Module.Features.Identities.AnonymizeCreatedByOfFiles;
using DeleteFilesOfIdentity = Backbone.Modules.Files.Module.Features.Identities.DeleteFilesOfIdentity;
using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Files.Module.Features.Identities.DeleteIdentity;

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
        await _mediator.Send(new DeleteFilesOfIdentity.Command { IdentityAddress = identityAddress });
        await _mediator.Send(new AnonymizeCreatedByOfFiles.Command { IdentityAddress = identityAddress });
        await _deletionProcessLogger.LogDeletion(identityAddress, "Files");
    }
}
