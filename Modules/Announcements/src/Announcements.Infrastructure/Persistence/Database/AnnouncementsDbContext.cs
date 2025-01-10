using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.Modules.Announcements.Domain.Entities;
using Backbone.Modules.Announcements.Infrastructure.Persistence.Database.ValueConverters;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Announcements.Infrastructure.Persistence.Database;

public class AnnouncementsDbContext : AbstractDbContextBase
{
    public AnnouncementsDbContext()
    {
    }

    public AnnouncementsDbContext(DbContextOptions<AnnouncementsDbContext> options, IEventBus eventBus) : base(options, eventBus)
    {
    }

    public AnnouncementsDbContext(DbContextOptions<AnnouncementsDbContext> options, IServiceProvider serviceProvider, IEventBus eventBus) : base(options, eventBus, serviceProvider)
    {
    }

    public virtual DbSet<Announcement> Announcements { get; set; } = null!;
    public virtual DbSet<AnnouncementRecipient> AnnouncementRecipients { get; set; } = null!;

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<AnnouncementId>().AreUnicode(false).AreFixedLength().HaveMaxLength(AnnouncementId.MAX_LENGTH).HaveConversion<AnnouncementIdEntityFrameworkValueConverter>();
        configurationBuilder.Properties<AnnouncementLanguage>().AreUnicode(false).AreFixedLength().HaveMaxLength(AnnouncementLanguage.LENGTH)
            .HaveConversion<AnnouncementLanguageEntityFrameworkValueConverter>();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("Announcements");

        builder.Entity<AnnouncementRecipient>(entity => { entity.HasKey(e => e.Id); });

        builder.ApplyConfigurationsFromAssembly(typeof(AnnouncementsDbContext).Assembly);
    }
}
