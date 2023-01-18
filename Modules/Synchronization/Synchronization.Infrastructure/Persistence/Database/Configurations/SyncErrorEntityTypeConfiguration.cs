using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Synchronization.Domain.Entities.Sync;

namespace Synchronization.Infrastructure.Persistence.Database.Configurations;

public class SyncErrorEntityTypeConfiguration : IEntityTypeConfiguration<SyncError>
{
    public void Configure(EntityTypeBuilder<SyncError> builder)
    {
        builder.HasIndex(x => new {x.SyncRunId, x.ExternalEventId}).IsUnique();

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ErrorCode).HasMaxLength(50);
    }
}
