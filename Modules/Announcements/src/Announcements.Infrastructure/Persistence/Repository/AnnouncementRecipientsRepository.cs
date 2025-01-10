using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Announcements.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Announcements.Domain.Entities;
using Backbone.Modules.Announcements.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Announcements.Infrastructure.Persistence.Repository;

public class AnnouncementRecipientsRepository : IAnnouncementRecipientsRepository
{
    private readonly DbSet<AnnouncementRecipient> _announcementRecipients;
    private readonly AnnouncementsDbContext _dbContext;
    private readonly IQueryable<AnnouncementRecipient> _readOnlyAnnouncementRecipients;

    public AnnouncementRecipientsRepository(AnnouncementsDbContext dbContext)
    {
        _dbContext = dbContext;
        _announcementRecipients = dbContext.AnnouncementRecipients;
        _readOnlyAnnouncementRecipients = dbContext.AnnouncementRecipients.AsNoTracking();
    }

    public Task Add(AnnouncementRecipient announcementRecipient, CancellationToken cancellationToken)
    {
        _announcementRecipients.Add(announcementRecipient);

        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<List<AnnouncementRecipient>> FindAll(CancellationToken cancellationToken)
    {
        return _readOnlyAnnouncementRecipients.IncludeAll(_dbContext).ToListAsync(cancellationToken);
    }
}
