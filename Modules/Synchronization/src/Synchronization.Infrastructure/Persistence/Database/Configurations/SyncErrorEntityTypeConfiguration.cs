using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Synchronization.Infrastructure.Persistence.Database.Configurations;

public class SyncErrorEntityTypeConfiguration : EntityEntityTypeConfiguration<SyncError>
{
    public override void Configure(EntityTypeBuilder<SyncError> builder)
    {
        base.Configure(builder);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ErrorCode).HasMaxLength(100);
        builder.Property(x => x.CreatedAt);

        builder.HasOne<ExternalEvent>().WithMany().HasForeignKey(x => x.ExternalEventId).OnDelete(DeleteBehavior.Cascade);
    }
}
