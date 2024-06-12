using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database.EntityConfigurations;

public class TierEntityTypeConfiguration : EntityEntityTypeConfiguration<Tier>
{
    public override void Configure(EntityTypeBuilder<Tier> builder)
    {
        base.Configure(builder);
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Name).IsUnique();
        builder.Property(x => x.CanBeManuallyAssigned).HasDefaultValue(true);
        builder.Property(x => x.CanBeUsedAsDefaultForClient).HasDefaultValue(true);
    }
}
