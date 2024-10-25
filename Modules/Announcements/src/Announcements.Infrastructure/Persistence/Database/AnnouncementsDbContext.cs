﻿using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.Modules.Announcements.Domain.Entities;
using Backbone.Modules.Announcements.Domain.Ids;
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
    public virtual DbSet<AnnouncementText> AnnouncementText { get; set; } = null!;

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<AnnouncementId>().AreUnicode(false).AreFixedLength().HaveMaxLength(AnnouncementId.MAX_LENGTH);
        configurationBuilder.Properties<AnnouncementTextId>().AreUnicode(false).AreFixedLength().HaveMaxLength(AnnouncementTextId.MAX_LENGTH);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("Messages");

        builder.ApplyConfigurationsFromAssembly(typeof(AnnouncementsDbContext).Assembly);
    }
}
