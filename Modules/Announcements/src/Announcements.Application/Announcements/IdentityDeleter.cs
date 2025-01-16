using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Announcements.Application.Announcements.Commands.AnonymizeRecipient;
using MediatR;

namespace Backbone.Modules.Announcements.Application.Announcements;

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
        await _mediator.Send(new AnonymizeRecipientForIdentityCommand(identityAddress));
        await _deletionProcessLogger.LogDeletion(identityAddress, "AnnouncementRecipients");
    }
}
