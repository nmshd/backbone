using Backbone.AdminApi.Infrastructure.Persistence.Models.Synchronization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.AdminApi.Infrastructure.Persistence.Database.EntityTypeConfigurations.Synchronization;

public class DatawalletEntityTypeConfiguration : IEntityTypeConfiguration<Datawallet>
{
    public void Configure(EntityTypeBuilder<Datawallet> builder)
    {
        builder.ToTable("Datawallets", "Synchronization", t => t.ExcludeFromMigrations());

        builder.HasKey(x => x.Id);
    }
}
