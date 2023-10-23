using Backbone.Devices.Domain.Aggregates.Tier;
using Backbone.Devices.Infrastructure.OpenIddict;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Devices.Infrastructure.Persistence.Database.EntityConfigurations;
public class CustomOpenIddictEntityFrameworkCoreApplicationEntityTypeConfiguration : IEntityTypeConfiguration<CustomOpenIddictEntityFrameworkCoreApplication>
{
    public void Configure(EntityTypeBuilder<CustomOpenIddictEntityFrameworkCoreApplication> builder)
    {
        builder
            .Property(x => x.DefaultTier)
            .HasMaxLength(TierId.MAX_LENGTH)
            .IsRequired();

        builder
            .Property(x => x.CreatedAt);

        builder
            .HasOne<Tier>().WithMany().HasForeignKey(x => x.DefaultTier)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
