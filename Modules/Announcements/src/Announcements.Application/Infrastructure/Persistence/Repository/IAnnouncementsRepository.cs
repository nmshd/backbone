using System.Linq.Expressions;
using Backbone.Modules.Announcements.Domain.Entities;

namespace Backbone.Modules.Announcements.Application.Infrastructure.Persistence.Repository;

public interface IAnnouncementsRepository
{
    Task Add(Announcement announcement, CancellationToken cancellationToken);
    Task<List<Announcement>> FindAll(CancellationToken cancellationToken);
    Task<List<Announcement>> FindAllForIdentityAddress(Expression<Func<Announcement, bool>> filter, CancellationToken cancellationToken);
    Task Update(List<Announcement> announcements, CancellationToken cancellationToken);
}
