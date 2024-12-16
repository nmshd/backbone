using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Announcements.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Announcements.Infrastructure.Persistence.Database.EntityTypeConfigurations;

public class AnnouncementsTextEntityTypeConfiguration : EntityEntityTypeConfiguration<AnnouncementText>
{
    public override void Configure(EntityTypeBuilder<AnnouncementText> builder)
    {
        base.Configure(builder);

        builder.HasKey(a => new { a.AnnouncementId, a.Language });

        builder.Property(a => a.Title);
        builder.Property(a => a.Body);
    }
}
