using Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Domain.Aggregates.Devices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Quotas.Jobs.ConsistencyCheck.Infrastructure.Persistence.Database.EntityConfigurations.Devices;
public class TierEntityTypeConfiguration : IEntityTypeConfiguration<Tier>
{
    public void Configure(EntityTypeBuilder<Tier> builder)
    {
        builder.ToTable("Tiers", "Devices", x => x.ExcludeFromMigrations());
        builder.HasKey(x => x.Id);
    }
}
