using MediatR;

namespace Backbone.Modules.Announcements.Application.Announcements.Commands.DeleteAnnouncementRecipients;

public record DeleteAnnouncementRecipientsCommand : IRequest
{
    public DeleteAnnouncementRecipientsCommand(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; }
}
