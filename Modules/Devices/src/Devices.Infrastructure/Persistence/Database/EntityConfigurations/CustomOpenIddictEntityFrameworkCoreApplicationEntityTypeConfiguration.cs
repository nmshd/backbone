using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Infrastructure.OpenIddict;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database.EntityConfigurations;
public class CustomOpenIddictEntityFrameworkCoreApplicationEntityTypeConfiguration : IEntityTypeConfiguration<CustomOpenIddictEntityFrameworkCoreApplication>
{
    public void Configure(EntityTypeBuilder<CustomOpenIddictEntityFrameworkCoreApplication> builder)
    {
        builder
            .Property(x => x.DefaultTier)
            .HasMaxLength(TierId.MAX_LENGTH);
    }
}
