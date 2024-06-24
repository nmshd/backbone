using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.EntityTypeConfigurations;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database.EntityConfigurations;

public class PnsRegistrationEntityTypeConfiguration : EntityEntityTypeConfiguration<PnsRegistration>
{
    public override void Configure(EntityTypeBuilder<PnsRegistration> builder)
    {
        base.Configure(builder);
        builder.HasKey(x => x.DeviceId);
        builder.Property(x => x.IdentityAddress).IsRequired();
        builder.Property(x => x.DevicePushIdentifier);
        builder.Property(x => x.Handle).IsRequired();
        builder.Property(x => x.UpdatedAt);
        builder.Property(x => x.AppId);
        builder.Property(x => x.Environment);
    }
}
