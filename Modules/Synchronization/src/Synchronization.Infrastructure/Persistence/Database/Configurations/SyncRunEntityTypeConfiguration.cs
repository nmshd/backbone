using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Synchronization.Infrastructure.Persistence.Database.Configurations;

public class SyncRunEntityTypeConfiguration : IEntityTypeConfiguration<SyncRun>
{
    public void Configure(EntityTypeBuilder<SyncRun> builder)
    {
        builder.HasIndex(x => new { x.CreatedBy, x.Index }).IsUnique();
        builder.HasIndex(x => new { x.CreatedBy, x.FinalizedAt });
        builder.HasIndex(x => x.CreatedBy);

        builder.Ignore(x => x.IsFinalized);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedAt);
        builder.Property(x => x.EventCount);
        builder.Property(x => x.CreatedBy);
        builder.Property(x => x.CreatedByDevice);

        builder.HasMany(x => x.ExternalEvents).WithOne(ee => ee.SyncRun).OnDelete(DeleteBehavior.Cascade);
    }
}
