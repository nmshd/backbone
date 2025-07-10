using MediatR;

namespace Backbone.Modules.Announcements.Application.Announcements.Commands.DeleteAnnouncementRecipients;

public record DeleteAnnouncementRecipientsCommand : IRequest
{
    public required string IdentityAddress { get; init; }
}
