using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Database.EntityConfigurations;

public class IdentityEntityTypeConfiguration : IEntityTypeConfiguration<Identity>
{
    public void Configure(EntityTypeBuilder<Identity> builder)
    {
        builder.HasKey(x => x.Address);

        builder.HasOne<Tier>().WithMany().HasForeignKey(x => x.TierId);

        builder.Property(x => x.Address).IsUnicode(false).IsFixedLength().HasMaxLength(IdentityAddress.MAX_LENGTH);
        builder.Property(x => x.TierId).IsUnicode(false).IsFixedLength().HasMaxLength(20);
        builder.HasMany<TierQuota>(x => x.TierQuotas).WithOne().HasForeignKey(x => x.ApplyTo);
    }
}
