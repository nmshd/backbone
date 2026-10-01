using MediatR;

namespace Backbone.Modules.Announcements.Module.Features.Announcements.DeleteAnnouncementRecipients;

public record DeleteAnnouncementRecipientsCommand : IRequest
{
    public required string IdentityAddress { get; init; }
}
