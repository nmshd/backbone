using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Announcements.Domain.Entities;

namespace Backbone.Modules.Announcements.Application.Infrastructure.Persistence.Repository;

public interface IAnnouncementRecipientRepository
{
    Task<List<AnnouncementRecipient>> FindAllForIdentityAddress(IdentityAddress identityAddress, CancellationToken cancellationToken);
    Task Update(List<AnnouncementRecipient> announcementRecipients, CancellationToken cancellationToken);
}
