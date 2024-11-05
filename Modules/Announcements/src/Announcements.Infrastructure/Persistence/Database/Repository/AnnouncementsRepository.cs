using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Announcements.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Announcements.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Announcements.Infrastructure.Persistence.Database.Repository;
public class AnnouncementsRepository : IAnnouncementsRepository
{
    private readonly AnnouncementsDbContext _dbContext;
    private readonly DbSet<Announcement> _announcements;

    public AnnouncementsRepository(AnnouncementsDbContext dbContext)
    {
        _dbContext = dbContext;
        _announcements = dbContext.Announcements;
    }

    public async Task Add(Announcement announcement, CancellationToken cancellationToken)
    {
        _announcements.Add(announcement);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<List<Announcement>> FindAll(CancellationToken cancellationToken)
    {
        return _announcements.IncludeAll(_dbContext).ToListAsync(cancellationToken);
    }
}
