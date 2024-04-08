using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database.EntityConfigurations;
public class TierEntityTypeConfiguration : IEntityTypeConfiguration<Tier>
{
    public void Configure(EntityTypeBuilder<Tier> builder)
    {
        builder.ToTable(nameof(Tier) + "s");
        builder.Property(x => x.Id);
        builder.Property(x => x.Name).IsUnicode(true).IsFixedLength(false).HasMaxLength(30);
        builder.HasMany(x => x.Quotas).WithOne().OnDelete(DeleteBehavior.Cascade);
    }
}
