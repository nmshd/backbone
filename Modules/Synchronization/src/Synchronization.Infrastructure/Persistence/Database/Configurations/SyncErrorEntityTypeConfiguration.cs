using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Synchronization.Infrastructure.Persistence.Database.Configurations;

public class SyncErrorEntityTypeConfiguration : EntityEntityTypeConfiguration<SyncError>
{
    public override void Configure(EntityTypeBuilder<SyncError> builder)
    {
        base.Configure(builder);

        builder.HasIndex(x => new { x.SyncRunId, x.ExternalEventId });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ErrorCode).HasMaxLength(100);

        builder.Property(x => x.SyncRunId);
    }
}
