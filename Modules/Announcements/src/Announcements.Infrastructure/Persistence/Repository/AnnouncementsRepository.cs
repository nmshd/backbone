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
    private readonly AnnouncementsDbContext _dbContext;
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
        return _readOnlyAnnouncements.IncludeAll(_dbContext).AsSplitQuery().ToListAsync(cancellationToken);
    }

    public Task<List<Announcement>> FindAllWhereIdentityAddressIs(Expression<Func<Announcement, bool>> filter, CancellationToken cancellationToken)
    {
        return _readOnlyAnnouncements
            .IncludeAll(_dbContext)
            .Where(filter)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
    }

    public Task Update(List<Announcement> announcements, CancellationToken cancellationToken)
    {
        _announcements.UpdateRange(announcements);
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
