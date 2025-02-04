using Backbone.Modules.Announcements.Domain.Entities;

namespace Backbone.Modules.Announcements.Application.Infrastructure.Persistence.Repository;

public interface IAnnouncementsRepository
{
    Task Add(Announcement announcement, CancellationToken cancellationToken);
    Task<List<Announcement>> FindAll(CancellationToken cancellationToken);
    Task<Announcement?> FindById(AnnouncementId id, CancellationToken cancellationToken);
}
