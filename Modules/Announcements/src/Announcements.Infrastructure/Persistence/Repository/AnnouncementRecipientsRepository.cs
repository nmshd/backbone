using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Announcements.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Announcements.Domain.Entities;
using Backbone.Modules.Announcements.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Announcements.Infrastructure.Persistence.Repository;

public class AnnouncementRecipientsRepository : IAnnouncementRecipientRepository
{
    private readonly DbSet<AnnouncementRecipient> _announcementRecipients;
    private readonly AnnouncementsDbContext _dbContext;

    public AnnouncementRecipientsRepository(AnnouncementsDbContext dbContext)
    {
        _dbContext = dbContext;
        _announcementRecipients = dbContext.AnnouncementRecipients;
    }


    public async Task<List<AnnouncementRecipient>> FindAllForIdentityAddress(IdentityAddress identityAddress, CancellationToken cancellationToken)
    {
        return await _announcementRecipients
            .Where(x => x.Address == identityAddress)
            .ToListAsync(cancellationToken);
    }

    public async Task Update(List<AnnouncementRecipient> announcementRecipients, CancellationToken cancellationToken)
    {
        _announcementRecipients.UpdateRange(announcementRecipients);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
