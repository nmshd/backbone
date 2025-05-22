using Backbone.AdminApi.Infrastructure.Persistence.Models.Synchronization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.AdminApi.Infrastructure.Persistence.Database.EntityTypeConfigurations.Synchronization;

public class DatawalletModificationEntityTypeConfiguration : IEntityTypeConfiguration<DatawalletModification>
{
    public void Configure(EntityTypeBuilder<DatawalletModification> builder)
    {
        builder.ToTable("DatawalletModifications", "Synchronization", t => t.ExcludeFromMigrations());

        builder.HasKey(x => x.Id);
    }
}
