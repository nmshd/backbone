using Backbone.AdminApi.Infrastructure.Persistence.Models.Synchronization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.AdminApi.Infrastructure.Persistence.Database.EntityTypeConfigurations.Synchronization;

public class SyncErrorEntityTypeConfiguration : IEntityTypeConfiguration<SyncError>
{
    public void Configure(EntityTypeBuilder<SyncError> builder)
    {
        builder.ToTable("SyncErrors", "Synchronization", t => t.ExcludeFromMigrations());

        builder.HasKey(x => x.Id);
    }
}
