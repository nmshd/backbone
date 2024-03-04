using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Synchronization.Infrastructure.Persistence.Database.Configurations;

public class SyncErrorEntityTypeConfiguration : IEntityTypeConfiguration<SyncError>
{
    public void Configure(EntityTypeBuilder<SyncError> builder)
    {
        builder.HasIndex(x => new { x.SyncRunId, x.ExternalEventId }).IsUnique();

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ErrorCode).HasMaxLength(50);
    }
}
