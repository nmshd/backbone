﻿using System.Linq.Expressions;
using Backbone.Modules.Announcements.Domain.Entities;

namespace Backbone.Modules.Announcements.Application.Infrastructure.Persistence.Repository;

public interface IAnnouncementsRepository
{
    Task Add(Announcement announcement, CancellationToken cancellationToken);
    Task<List<Announcement>> List(CancellationToken cancellationToken);
    Task<List<Announcement>> List(Expression<Func<Announcement, bool>> filter, CancellationToken cancellationToken);
    Task DeleteRecipients(Expression<Func<AnnouncementRecipient, bool>> filter, CancellationToken cancellationToken);
    Task<Announcement?> Get(AnnouncementId id, CancellationToken cancellationToken);
    Task<int> Delete(AnnouncementId id, CancellationToken cancellationToken);
}
