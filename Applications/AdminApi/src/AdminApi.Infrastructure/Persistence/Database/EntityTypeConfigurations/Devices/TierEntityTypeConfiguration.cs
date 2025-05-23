using Backbone.AdminApi.Infrastructure.Persistence.Models.Devices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.AdminApi.Infrastructure.Persistence.Database.EntityTypeConfigurations.Devices;

public class TierEntityTypeConfiguration : IEntityTypeConfiguration<Tier>
{
    public void Configure(EntityTypeBuilder<Tier> builder)
    {
        builder.ToTable("Tiers", "Devices", t => t.ExcludeFromMigrations());

        builder.HasKey(x => x.Id);
    }
}
