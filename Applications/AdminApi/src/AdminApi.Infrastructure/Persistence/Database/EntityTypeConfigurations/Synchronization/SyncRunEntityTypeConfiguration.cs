using Backbone.AdminApi.Infrastructure.Persistence.Models.Synchronization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.AdminApi.Infrastructure.Persistence.Database.EntityTypeConfigurations.Synchronization;

public class SyncRunEntityTypeConfiguration : IEntityTypeConfiguration<SyncRun>
{
    public void Configure(EntityTypeBuilder<SyncRun> builder)
    {
        builder.ToTable("SyncRuns", "Synchronization", t => t.ExcludeFromMigrations());

        builder.HasKey(x => x.Id);
    }
}
