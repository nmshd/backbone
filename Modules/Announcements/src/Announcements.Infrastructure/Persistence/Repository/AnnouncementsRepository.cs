using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Announcements.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Announcements.Domain.Entities;
using Backbone.Modules.Announcements.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Announcements.Infrastructure.Persistence.Repository;

public class AnnouncementsRepository : IAnnouncementsRepository
{
    private readonly DbSet<Announcement> _announcements;
    private readonly DbSet<AnnouncementRecipient> _announcementRecipients;
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

    public Task<List<Announcement>> FindAll(CancellationToken cancellationToken)
    {
        return _readOnlyAnnouncements.IncludeAll(_dbContext).AsSplitQuery().ToListAsync(cancellationToken);
    }

    public Task DeleteRecipients(Expression<Func<AnnouncementRecipient, bool>> filter, CancellationToken cancellationToken)
    {
        return _announcementRecipients
            .IncludeAll(_dbContext)
            .Where(filter)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
