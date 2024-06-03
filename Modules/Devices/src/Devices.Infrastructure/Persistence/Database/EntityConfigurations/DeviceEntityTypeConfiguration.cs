using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database.EntityConfigurations;

public class DeviceEntityTypeConfiguration : EntityEntityTypeConfiguration<Device>
{
    public override void Configure(EntityTypeBuilder<Device> builder)
    {
        base.Configure(builder);
        builder.Ignore(x => x.IsOnboarded);
        builder.Property(x => x.CommunicationLanguage).HasDefaultValue(CommunicationLanguage.DEFAULT_LANGUAGE);
    }
}
