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

    public AnnouncementsDbContext(DbContextOptions<AnnouncementsDbContext> options, IServiceProvider serviceProvider) : base(options, serviceProvider)
    {
    }

    public AnnouncementsDbContext(DbContextOptions<AnnouncementsDbContext> options) : base(options)
    {
    }

    public virtual DbSet<Announcement> Announcements { get; set; } = null!;
    public virtual DbSet<AnnouncementRecipient> AnnouncementRecipients { get; set; } = null!;

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<AnnouncementId>().AreUnicode(false).AreFixedLength().HaveMaxLength(AnnouncementId.MAX_LENGTH).HaveConversion<AnnouncementIdEntityFrameworkValueConverter>();
        configurationBuilder.Properties<AnnouncementActionId>().AreUnicode(false).AreFixedLength().HaveMaxLength(AnnouncementActionId.MAX_LENGTH)
            .HaveConversion<AnnouncementActionIdEntityFrameworkValueConverter>();
        configurationBuilder.Properties<AnnouncementLanguage>().AreUnicode(false).AreFixedLength().HaveMaxLength(AnnouncementLanguage.LENGTH)
            .HaveConversion<AnnouncementLanguageEntityFrameworkValueConverter>();
        configurationBuilder.Properties<AnnouncementIqlQuery>().AreUnicode(true).AreFixedLength(false).HaveMaxLength(AnnouncementIqlQuery.MAX_LENGTH)
            .HaveConversion<AnnouncementIqlQueryEntityFrameworkValueConverter>();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("Announcements");

        builder.ApplyConfigurationsFromAssembly(typeof(AnnouncementsDbContext).Assembly);
    }
}
