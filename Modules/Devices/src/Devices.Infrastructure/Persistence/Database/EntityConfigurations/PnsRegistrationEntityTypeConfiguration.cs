using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database.EntityConfigurations;
public class PnsRegistrationEntityTypeConfiguration : IEntityTypeConfiguration<PnsRegistration>
{
    public void Configure(EntityTypeBuilder<PnsRegistration> builder)
    {
        builder.HasKey(x => x.DeviceId);
        builder.Property(x => x.IdentityAddress).IsRequired();
        builder.Property(x => x.Handle).IsRequired();
        builder.Property(x => x.UpdatedAt);
        builder.Property(x => x.AppId);
    }
}
