using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database.EntityConfigurations;

public class FeatureFlagEntityTypeConfiguration : EntityEntityTypeConfiguration<FeatureFlag>
{
    public override void Configure(EntityTypeBuilder<FeatureFlag> builder)
    {
        base.Configure(builder);

        builder.HasKey(x => new { x.Name, x.OwnerAddress });

        builder.Property(x => x.IsEnabled);
    }
}
