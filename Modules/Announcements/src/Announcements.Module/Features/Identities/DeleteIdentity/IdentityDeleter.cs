using DeleteAnnouncementRecipients = Backbone.Modules.Announcements.Module.Features.Announcements.DeleteAnnouncementRecipients;
using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Announcements.Module.Features.Identities.DeleteIdentity;

public class IdentityDeleter : IIdentityDeleter
{
    private readonly IDeletionProcessLogger _deletionProcessLogger;
    private readonly IMediator _mediator;

    public IdentityDeleter(IMediator mediator, IDeletionProcessLogger deletionProcessLogger)
    {
        _mediator = mediator;
        _deletionProcessLogger = deletionProcessLogger;
    }

    public async Task Delete(IdentityAddress identityAddress)
    {
        await _mediator.Send(new DeleteAnnouncementRecipients.Command { IdentityAddress = identityAddress });
        await _deletionProcessLogger.LogDeletion(identityAddress, "AnnouncementRecipients");
    }
}
