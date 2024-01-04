using Backbone.AdminUi.Infrastructure.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.AdminUi.Infrastructure.Persistence.Database.EntityTypeConfigurations;
public class MessageOverviewEntityTypeConfiguration : IEntityTypeConfiguration<MessageOverview>
{
    public void Configure(EntityTypeBuilder<MessageOverview> builder)
    {
        builder.ToView("MessageOverviews");
        builder
            .HasMany(m => m.Recipients)
            .WithOne()
            .HasForeignKey(r => r.MessageId)
            .IsRequired();

        builder.HasKey(m => m.MessageId);
    }
}
