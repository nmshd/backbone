using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database.EntityConfigurations;

public class IdentityEntityTypeConfiguration : IEntityTypeConfiguration<Identity>
{
    public void Configure(EntityTypeBuilder<Identity> builder)
    {
        builder.HasKey(x => x.Address);
        builder.HasOne<Tier>().WithMany().HasForeignKey(x => x.TierId).OnDelete(DeleteBehavior.NoAction);
        builder.HasMany(x => x.TierQuotas).WithOne().HasForeignKey(x => x.ApplyTo);
        builder.HasMany(x => x.IndividualQuotas).WithOne().HasForeignKey(x => x.ApplyTo);
        builder.Property(x => x.Address).IsUnicode(false).IsFixedLength().HasMaxLength(IdentityAddress.MAX_LENGTH);
        builder.Property(x => x.TierId).IsUnicode(false).IsFixedLength().HasMaxLength(20);
        builder.Ignore(x => x.AllQuotas);
    }
}
