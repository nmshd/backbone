using MediatR;

namespace Backbone.Modules.Announcements.Module.Features.Announcements.DeleteAnnouncementRecipients;

public record Command : IRequest
{
    public required string IdentityAddress { get; init; }
}
