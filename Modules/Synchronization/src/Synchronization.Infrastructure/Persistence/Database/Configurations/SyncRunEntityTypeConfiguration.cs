using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Synchronization.Infrastructure.Persistence.Database.Configurations;

public class SyncRunEntityTypeConfiguration : EntityEntityTypeConfiguration<SyncRun>
{
    public override void Configure(EntityTypeBuilder<SyncRun> builder)
    {
        base.Configure(builder);

        builder.HasIndex(x => new { x.CreatedBy, x.Index }).IsUnique();
        builder.HasIndex(x => new { x.CreatedBy, x.FinalizedAt });

        builder.Ignore(x => x.IsFinalized);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedAt);
        builder.Property(x => x.EventCount);
        builder.Property(x => x.CreatedBy);
        builder.Property(x => x.CreatedByDevice);

        builder.HasMany(x => x.ExternalEvents).WithOne(x => x.SyncRun).OnDelete(DeleteBehavior.Cascade);

        // We have to configure SetNull because
        // - sync errors with an error count < 2 are deleted automatically because the corresponding external events are deleted, which cascades to the related errors
        // - sync errors with an error count = 2 are kept for inspection, but the foreign key to the sync run is removed
        builder.HasMany(x => x.Errors).WithOne(x => x.SyncRun).OnDelete(DeleteBehavior.SetNull);
    }
}
