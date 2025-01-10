using Backbone.Modules.Announcements.Domain.Entities;

namespace Backbone.Modules.Announcements.Application.Infrastructure.Persistence.Repository;

public interface IAnnouncementRecipientsRepository
{
    Task Add(AnnouncementRecipient announcementRecipient, CancellationToken cancellationToken);
    Task<List<AnnouncementRecipient>> FindAll(CancellationToken cancellationToken);
}
