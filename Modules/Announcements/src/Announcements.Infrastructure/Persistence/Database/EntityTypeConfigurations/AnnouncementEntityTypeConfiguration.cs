using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Announcements.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Announcements.Infrastructure.Persistence.Database.EntityTypeConfigurations;

public class AnnouncementEntityTypeConfiguration : EntityEntityTypeConfiguration<Announcement>
{
    public override void Configure(EntityTypeBuilder<Announcement> builder)
    {
        base.Configure(builder);

        builder.HasKey(a => a.Id);

        builder.Property(a => a.CreatedAt);
        builder.Property(a => a.ExpiresAt);
        builder.Property(a => a.Severity);
        builder.Property(a => a.IqlQuery);

        builder.HasMany(a => a.Texts).WithOne();
    }
}
