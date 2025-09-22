using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Announcements.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Announcements.Domain.Entities;
using Backbone.Modules.Announcements.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Announcements.Infrastructure.Persistence.Repository;

public class AnnouncementsRepository : IAnnouncementsRepository
{
    private readonly DbSet<AnnouncementRecipient> _announcementRecipients;
    private readonly DbSet<Announcement> _announcements;
    private readonly AnnouncementsDbContext _dbContext;
    private readonly IQueryable<Announcement> _readOnlyAnnouncements;

    public AnnouncementsRepository(AnnouncementsDbContext dbContext)
    {
        _dbContext = dbContext;
        _announcements = dbContext.Announcements;
        _announcementRecipients = dbContext.AnnouncementRecipients;
        _readOnlyAnnouncements = dbContext.Announcements.AsNoTracking();
    }

    public async Task Add(Announcement announcement, CancellationToken cancellationToken)
    {
        _announcements.Add(announcement);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<List<Announcement>> List(CancellationToken cancellationToken)
    {
        return _readOnlyAnnouncements.IncludeAll(_dbContext).AsSplitQuery().ToListAsync(cancellationToken);
    }

    public Task<List<Announcement>> List(Expression<Func<Announcement, bool>> filter, CancellationToken cancellationToken)
    {
        return _readOnlyAnnouncements
            .IncludeAll(_dbContext)
            .AsSplitQuery()
            .Where(filter)
            .ToListAsync(cancellationToken);
    }

    public Task DeleteRecipients(Expression<Func<AnnouncementRecipient, bool>> filter, CancellationToken cancellationToken)
    {
        return _announcementRecipients
            .Where(filter)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public Task<Announcement?> Get(AnnouncementId id, CancellationToken cancellationToken)
    {
        return _readOnlyAnnouncements.IncludeAll(_dbContext).AsSplitQuery().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<int> Delete(AnnouncementId id, CancellationToken cancellationToken)
    {
        return await _announcements.Where(a => a.Id == id).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<int> Delete(Expression<Func<Announcement, bool>> filter, CancellationToken cancellationToken)
    {
        return await _announcements.Where(filter).ExecuteDeleteAsync(cancellationToken);
    }
}
