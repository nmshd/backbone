using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Announcements.Domain.Entities;
using Backbone.Modules.Announcements.Infrastructure.Persistence.Database.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Announcements.Infrastructure.Persistence.Database.EntityTypeConfigurations;

public class AnnouncementActionEntityTypeConfiguration : EntityEntityTypeConfiguration<AnnouncementAction>
{
    public override void Configure(EntityTypeBuilder<AnnouncementAction> builder)
    {
        base.Configure(builder);

        builder.ToTable("AnnouncementActions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Link).HasMaxLength(300);

        builder.Property(x => x.Order);

        builder.Property(x => x.DisplayName).HasConversion<AnnouncementActionDisplayNameEntityFrameworkConverter>(ValueComparer.CreateDefault<Dictionary<AnnouncementLanguage, string>>(false))
            .HasMaxLength(300);
    }
}
