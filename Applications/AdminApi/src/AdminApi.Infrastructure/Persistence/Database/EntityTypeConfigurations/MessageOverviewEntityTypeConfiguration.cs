using Backbone.AdminApi.Infrastructure.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.AdminApi.Infrastructure.Persistence.Database.EntityTypeConfigurations;

public class MessageOverviewEntityTypeConfiguration : IEntityTypeConfiguration<MessageOverview>
{
    public void Configure(EntityTypeBuilder<MessageOverview> builder)
    {
        builder.ToView("MessageOverviews", "AdminUi");
        builder
            .HasMany(m => m.Recipients)
            .WithOne()
            .IsRequired();

        builder.HasKey(m => m.MessageId);
    }
}
