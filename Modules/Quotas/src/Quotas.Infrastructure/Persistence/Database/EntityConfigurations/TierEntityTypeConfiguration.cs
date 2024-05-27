using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database.EntityConfigurations;

public class TierEntityTypeConfiguration : EntityEntityTypeConfiguration<Tier>
{
    public override void Configure(EntityTypeBuilder<Tier> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Id);
        builder.Property(x => x.Name).IsUnicode(true).IsFixedLength(false).HasMaxLength(30);
        builder.HasMany(x => x.Quotas).WithOne().OnDelete(DeleteBehavior.Cascade);
    }
}
