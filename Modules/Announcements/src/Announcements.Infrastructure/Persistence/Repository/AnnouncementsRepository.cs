using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Announcements.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Announcements.Domain.Entities;
using Backbone.Modules.Announcements.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Announcements.Infrastructure.Persistence.Repository;

public class AnnouncementsRepository : IAnnouncementsRepository
{
    private readonly AnnouncementsDbContext _dbContext;
    private readonly DbSet<Announcement> _announcements;
    private readonly IQueryable<Announcement> _readOnlyAnnouncements;

    public AnnouncementsRepository(AnnouncementsDbContext dbContext)
    {
        _dbContext = dbContext;
        _announcements = dbContext.Announcements;
        _readOnlyAnnouncements = dbContext.Announcements.AsNoTracking();
    }

    public async Task Add(Announcement announcement, CancellationToken cancellationToken)
    {
        _announcements.Add(announcement);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<List<Announcement>> FindAll(CancellationToken cancellationToken)
    {
        return _readOnlyAnnouncements.IncludeAll(_dbContext).ToListAsync(cancellationToken);
    }
}
