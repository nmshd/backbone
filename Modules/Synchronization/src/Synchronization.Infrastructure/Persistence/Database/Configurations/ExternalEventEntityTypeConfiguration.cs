using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Backbone.Modules.Synchronization.Infrastructure.Persistence.Database.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Synchronization.Infrastructure.Persistence.Database.Configurations;

public class ExternalEventEntityTypeConfiguration : EntityEntityTypeConfiguration<ExternalEvent>
{
    public override void Configure(EntityTypeBuilder<ExternalEvent> builder)
    {
        base.Configure(builder);

        builder.HasIndex(x => new { x.Owner, x.Index }).IsUnique();
        builder.HasIndex(x => new { x.Owner, x.SyncRunId });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Type).HasMaxLength(50);
        builder.Property(x => x.CreatedAt);

        builder.Property(x => x.Payload)
            .HasMaxLength(200)
            .HasConversion<ExternalEventPayloadEntityFrameworkValueConverter>();

        builder.Property(x => x.Context).HasMaxLength(20);

        builder.Property(x => x.IsDeliveryBlocked);

        builder.HasMany(x => x.Errors)
            .WithOne()
            .HasForeignKey(x => x.ExternalEventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
